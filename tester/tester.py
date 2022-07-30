#! /bin/python3

import socket
import time
import json
import threading
import re
import os
import sys
import random

total_test_num = 19  # TODO Manually fix this
PORT = 11000
HOST = 'localhost'

# Global list of socket tuples (socket object, ID) used during multithreading.
connections = []

connections_lock = threading.Lock()

# These messages should match the error messages that the server gives
UNDO_ERROR = "can't undo"
REVERT_ERROR = "can't revert"
CIRC_DEPENDENCY_ERROR = "circular dependency"



def fail():

    print('fail')
    os._exit(1)

def succeed():

    print('pass')
    os._exit(1)

def time_limit(seconds, fail=True):

    time.sleep(seconds)

    if fail:
        print('fail: exceed time limit')
        os._exit(1)
    else:
        succeed()


def test_info(max_time, test_name):

    print('Max test time: {} seconds'.format(max_time))
    print(test_name)


def parse_json(data):
    """Returns a list where each element is dictionary holding JSON information \
    FIX: security bug if user includes `}` in string contents of cell"""
    if data == "":
        return []

    res = []
    for msg in data.split("\n") :
        if re.search('^\{.+\}$',msg) is not None:
            res.append(json.loads(msg))

    return res


def connection(username='username', rand_sheet_choice=False, sheet_name=""):
    '''
    Forms a connection to the server and returns a tuple containing the socket and client ID

    Parameters:
        username: username to provide to the server
        rand_sheet_choice: Boolean that determines if the choice of spreadsheet is randomized. If set to false,
            choose the first spreadsheet that the server lists.
        sheet_name: String to define a spreadsheet name to attempt to open. If left empty, uses a standard
            default string.
    Returns:
        tuple in the format: (socket object, client ID, client username, sheet_name)
    '''
    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    s.connect((HOST, PORT))

    # Send username for first part of handshake
    s.sendall((username+'\n').encode())

    # receive list of files
    data = ""
    while not (len(data) >= 2 and data[-2:] == '\n\n'):
        data += s.recv(1024).decode('utf-8')

    # open a spreadsheet
    if sheet_name == "":
        if len(data) == 0:
            sheet_name = "new_sheet"
        else:
            if rand_sheet_choice:
                sheets = data.split("\n")
                sheet_name = random.choice(sheets)
            else:
                sheet_name = data.split("\n")[0]

    sheet_name += '\n'

    s.sendall(sheet_name.encode())

    # receive cell updates and user id from server
    messages = ""
    while True:
        # listen for cell updates
        messages = messages + s.recv(1024).decode('utf-8')
        match = re.search("(\d+)\n", messages)
        if match is not None:
            return (s, int(match.group()), username)
        else: #if messages[-1] == '\n':
            try:
                parse_json(messages)
            except:
                print(messages)
                fail()

def valid_json(message:dict, values):
    """
    Checks if a JSON message is in valid format
    message is JSON message that has been converted into a Python dictionary
    values is list of field values. \
    Returns: true if it is, false if it isn't
    """
    jsons = {
        "cellUpdated": ["messageType", "cellName", "contents"],
        "cellSelected": ["messageType", "cellName", "selector", "selectorName"],
        "disconnected": ["messageType", "user"],
        "requestError": ["messageType", "cellName", "message"],
        "serverError": ["messageType", "message"]
    }

    if values[0] != message["messageType"]:
        return False


    # special cases for error types since there is no protocal defined req for message feild
    if message["messageType"] in ["serverError", "requestError"]:
        if values[0] == "serverError" and message["message"] != "":
            return True
        elif values[0] == "requestError" == message["messageType"] and message["cellName"]==values[1] and message["message"] != "":
            return True

    fields = jsons[values[0]]

    if len(fields) != len(message):
        return False

    for i, field in enumerate(fields):
        if field not in message.keys():
            return False
        else:
            if message[field] != values[i]:
                return False

    return True



def convert_encode(dic):
    """Accepts a python dictionary and returns an encoded message that is a JSON with newline"""
    return (json.dumps(dic)+"\n").encode()

def test_1():
    """Tests receiving of spreadsheet files"""
    try:
        time_out = 3
        test_info(time_out, 'Handshake 1 test')
        timer_thread = threading.Thread(target=time_limit, args=([time_out]))
        timer_thread.start()

        with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
            s.connect((HOST, PORT))

            # Send username for first part of handshake
            s.sendall(b'username\n')

            # receive list of files
            data = ""
            while not (len(data) >= 2 and data[-2:] == '\n\n'):
                data += s.recv(1024).decode('utf-8')

            # print("received from server:\n"+data)

        if data[-1] != '\n':
            fail()

        if len(data) > 1 and data[-2] != '\n':
            fail()

        succeed()

    except:
        fail()


def test_2():
    """Tries to open a spreadsheet out of the ones that the server gives in the handshake"""
    try:
        time_out = 3
        test_info(time_out, 'Open spreadsheet test')
        timer_thread = threading.Thread(target=time_limit, args=([time_out]))
        timer_thread.start()

        with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
            s.connect((HOST, PORT))

            # Send username for first part of handshake
            s.sendall(b'username\n')

            # receive list of files
            data = ""
            while not (len(data) >= 2 and data[-2:] == '\n\n'):
                data += s.recv(1024).decode('utf-8')

            # open a spreadsheet
            sheet_name = ""
            if len(data) == 0:
                sheet_name = "new_sheet\n"
            else:
                sheet_name = data.split("\n")[0] + '\n'

            s.sendall(sheet_name.encode())

            # receive cell updates and user id from server
            messages = ""
            while True:
                # listen for cell updates
                messages = messages + s.recv(1024).decode('utf-8')

                match = re.search("(\d+)\n", messages)
                if match is not None:
                    succeed()
                    try:
                        parse_json(messages[:-2])
                    except:
                        print(messages)
                        fail()

                    succeed()

    except:
        fail()


def test_3():
    """Tests requesting of invalid spreadsheet name"""
    time_out = 3
    test_info(time_out, 'Open invalid spreadsheet test')

    timer_thread = threading.Thread(target=time_limit, args=([time_out]))
    timer_thread.start()

    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
        s.connect((HOST, PORT))

        # Send username for first part of handshake
        s.sendall(b'Username\n')

        # receive list of files
        data = ""
        while not (len(data) >= 2 and data[-2:] == '\n\n'):
            data += s.recv(1024).decode('utf-8')

        data = data.replace('\n', '') + 'a\n'

        s.sendall(data.encode('utf-8'))

        # get cell id
        messages = s.recv(1024).decode('utf-8')

        if len(messages) >= 2 and messages[-2].isdigit() and messages[-1] == '\n':
            succeed()
        else:
            fail()

def test_4():
    """Select cell test (w/ 2 clients)"""
    try:
        test_info(5, 'Select cell test (w/ 2 clients)')
        timer_thread = threading.Thread(target=time_limit, args=([5]))
        timer_thread.start()

        s1, uid1, uname1 = connection()
        s2, uid2, uname2 = connection('user2')

        select_json = {"requestType": "selectCell", "cellName": "Z99"}
        s1.sendall(convert_encode(select_json))

        data = ""
        while True:
            data += s2.recv(1024).decode('utf-8')
            messages = parse_json(data)
            for msg in messages:
                if valid_json(msg, ["cellSelected", "Z99", uid1, uname1]):
                    succeed()

    except:
        e = sys.exc_info()[0]
        print("  >> ERROR :: exception occured !!  [test_4]\n"+
              "  >>            error: " + e)
        fail()


def test_5():
    """Disconnect test"""
    try:
        test_info(5, 'Disconnect test')
        timer_thread = threading.Thread(target=time_limit, args=([5]))
        timer_thread.start()

        s1, s1_id, s1_name = connection()
        s2, s2_id, s2_name = connection('user2')

        s1.close()

        data = ""
        while True:
            data += s2.recv(1024).decode('utf-8')
            messages = parse_json(data)
            for msg in messages:
                if valid_json(msg, ["disconnected", s1_id]):
                    succeed()

    except:
        fail()


def test_6():
    """Cell update test"""
    try:
        test_info(6, 'Cell update test')
        timer_thread = threading.Thread(target=time_limit, args=([5]))
        timer_thread.start()

        s1, s1_id, s1_name = connection()
        s2, s2_id, s2_name = connection('user2')

        data = ""

        select_json = {"requestType": "selectCell", "cellName": "A1"}
        s1.sendall(convert_encode(select_json))

        s2.recv(1024).decode('utf-8')

        update_json = {"requestType": "editCell",
                       "cellName": "A1", "contents": "Hello"}
        s1.sendall(convert_encode(update_json))

        while True:
            data += s2.recv(1024).decode('utf-8')
            messages = parse_json(data)

            # print("message being processes:\n", data)

            for message in messages:
                if valid_json(message, ["cellUpdated", "A1", "Hello"]):
                    succeed()
                else:
                    continue

    except:
        e = sys.exc_info()[0]
        print("  >> ERROR :: exception occured !!  [test_4]\n"+
              "  >>            error: " + e)
        fail()


def test_7():
    """Undo test"""
    try:
        time_out = 5
        test_info(time_out, 'Undo test')
        timer_thread = threading.Thread(target=time_limit, args=([time_out]))
        timer_thread.start()

        s1, s1_id, s1_name = connection()

        select_json = {"requestType": "selectCell", "cellName": "A1"}
        s1.sendall(convert_encode(select_json))

        update_json = {"requestType": "editCell",
                     "cellName": "A1", "contents": ""}
        s1.sendall(convert_encode(update_json))

        update_json = {"requestType": "editCell",
                       "cellName": "A1", "contents": "Hello"}
        s1.sendall(convert_encode(update_json))

        undo_json = {"requestType": "undo"}
        s1.sendall(convert_encode(undo_json))

        data = ""
        while True:
            data += s1.recv(1024).decode('utf-8')

            messages = parse_json(data)
            # print(data)
            for message in messages:
                if valid_json(message, ["cellUpdated", "A1", ""]):
                    succeed()
    except:
        fail()


def test_8():
    """Revert test"""
    try:
        time_out = 3
        test_info(time_out, 'Revert test')
        timer_thread = threading.Thread(target=time_limit, args=([time_out]))
        timer_thread.start()

        s1, s1_id, s1_name = connection()

        select_json = {"requestType": "selectCell", "cellName": "A1"}
        s1.sendall(convert_encode(select_json))

        update_json = {"requestType": "editCell",
                     "cellName": "A1", "contents": ""}
        s1.sendall(convert_encode(update_json))

        update_json = {"requestType": "editCell",
                       "cellName": "A1", "contents": "Hello"}
        s1.sendall(convert_encode(update_json))

        revert_json = {"requestType": "revertCell", "cellName": "A1"}
        s1.sendall(convert_encode(revert_json))

        data = ""
        while True:
            data += s1.recv(1024).decode('utf-8')
            messages = parse_json(data)
            for message in messages:
                if valid_json(message, ["cellUpdated", "A1", ""]):
                    succeed()

    except:
        e = sys.exc_info()[0]
        print("  >> ERROR :: exception occured !!  [test_4]\n"+
              "  >>            error: " + e)
        fail()


def test_9():
    """Invalid request error test 1 - with undo"""
    try:
        time_out = 3
        test_info(time_out, 'Invalid request error test 1 - with undo')
        timer_thread = threading.Thread(target=time_limit, args=([time_out]))
        timer_thread.start()

        s1, s1_id, s1_name = connection(sheet_name="test_9")

        undo_json = {"requestType": "undo"}
        s1.sendall(convert_encode(undo_json))

        data = ""
        while True:
            data += s1.recv(1024).decode('utf-8')
            messages = parse_json(data)

            for message in messages:
                if valid_json(message, ["requestError", ""]):
                    succeed()

    except:
        e = sys.exc_info()[0]
        print("  >> ERROR :: exception occured !!  [test_9]\n"+
              "  >>            error: " + e)
        fail()


def test_10():
    """invalid request error test 2 - with revert"""
    try:
        time_out = 3
        test_info(time_out, 'Invalid request error test 2 - with revert')
        timer_thread = threading.Thread(target=time_limit, args=([time_out]))
        timer_thread.start()

        s1, s1_id, s1_name = connection(sheet_name="test_10")

        select_json = {"requestType": "selectCell", "cellName": "A1"}
        s1.sendall(convert_encode(select_json))

        undo_json = {"requestType": "revert", "cellName": "A1"}
        s1.sendall(convert_encode(undo_json))

        data = ""
        while True:
            data += s1.recv(1024).decode('utf-8')
            messages = parse_json(data)

            for message in messages:
                if valid_json(message, ["requestError", "A1", REVERT_ERROR]):
                    succeed()

    except:
        e = sys.exc_info()[0]
        print("  >> ERROR :: exception occured !!  [test_10]\n"+
              "  >>            error: " + e)
        fail()


def test_11():
    """Invalid request error test 3 - with edit causing circular dependency"""
    try:
        time_out = 3
        test_info(
            time_out, 'Invalid request error test 3 - with edit causing circular dependency')
        timer_thread = threading.Thread(target=time_limit, args=([time_out]))
        timer_thread.start()

        s1, s1_id, s1_name = connection()

        select_json = {"requestType": "selectCell", "cellName": "A1"}
        s1.sendall(convert_encode(select_json))

        update_json = {"requestType": "editCell",
                       "cellName": "A1", "contents": "=A1+10"}
        s1.sendall(convert_encode(update_json))

        data = ""
        while True:
            data += s1.recv(1024).decode('utf-8')
            messages = parse_json(data)
            for message in messages:
                if valid_json(message, ["requestError", "A1", CIRC_DEPENDENCY_ERROR]):
                    succeed()
    except:
        e = sys.exc_info()[0]
        print("  >> ERROR :: exception occured !!  [test_11]\n"+
              "  >>            error: " + e)
        fail()


def test_19():
    """Invalid request error test 3 - with edit causing circular dependency w/ 3 cells in circ"""
    try:
        time_out = 3
        test_info(
            time_out, 'Invalid request error test 3 - edit causing circular dependency (w/ 3 cells in circ depend)')
        timer_thread = threading.Thread(target=time_limit, args=([time_out]))
        timer_thread.start()

        s1, s1_id, s1_name = connection()

        update_json = {"requestType": "editCell",
                       "cellName": "A1", "contents": "=A2+10"}
        s1.sendall(convert_encode(update_json))

        update_json = {"requestType": "editCell",
                       "cellName": "A2", "contents": "=A3+10"}
        s1.sendall(convert_encode(update_json))

        update_json = {"requestType": "editCell",
                       "cellName": "A3", "contents": "=A1+10"}
        s1.sendall(convert_encode(update_json))

        data = ""
        while True:
            data += s1.recv(1024).decode('utf-8')
            messages = parse_json(data)

            for message in messages:
                if valid_json(message, ["requestError", "A3", CIRC_DEPENDENCY_ERROR]):
                 succeed()
    except:
        e = sys.exc_info()[0]
        print("  >> ERROR :: exception occured !!  [test_101]\n"+
              "  >>            error: " + e)
        fail()


def test_12():
    """Stress test - connections"""
    try:
        stress_level = 30
        time_out = 12
        test_info(time_out, f'Stress test - connections (x{stress_level})')
        timer_thread = threading.Thread(target=time_limit, args=([time_out]))
        timer_thread.start()

        def make_conn():
            connection("user" + str(i))

        threads = []
        for i in range(stress_level):
            threads.append(threading.Thread(target=make_conn))
            threads[i].start()

        for t in threads:
            t.join()

        succeed()

    except:
        e = sys.exc_info()[0]
        print("  >> ERROR :: exception occured !!  [test_101]\n"+
              "  >>            error: " + e)
        fail()


def test_13():
    """Stress test - disconnecting"""

    global connections

    time_out = 30
    client_count = 16

    try:
        test_pass = []
        test_info(time_out, 'Stress test - disconnection')
        timer_thread = threading.Thread(target=time_limit, args=([time_out]))
        timer_thread.start()

        connections = []

        # Connect 30 client sockets to server
        multiple_connection(client_count, 2)



        def listen_disconnect(listener_socket, disconnected_id, listener_id):
            """Helper to listen for a disconnect message triggered by the first socket closing.
            If a valid disconnect message is never received, stay in the while loop until timeout
                which fails the test."""

            data = ""
            while True:
                data += listener_socket.recv(1024).decode('utf-8')
                messages = parse_json(data)

                for msg in messages:
                    # print(f"l_id: {listener_id:<8d} d_id: {disconnected_id} \n {msg}\n")
                    if msg["messageType"] == "disconnected":
                        test_pass.append(msg["user"] == disconnected_id)
                        # listener_socket.close()
                        return

        # Listen for disconnect message in each thread
        threads = []
        for i,conn in enumerate(connections):
            threads.append(threading.Thread(target=listen_disconnect, args=([conn[0][0], conn[1][1], conn[0][1]])))
            threads[i].start()
            time.sleep(1)
            conn[1][0].close()

        # Wait for each thread to finish. Threads only finish when they receive a valid disconnect message.
        # If any threads are stuck in a loop, then the test fails by timeout.
        for t in threads:
            t.join()

        if client_count//2 == len(test_pass) and all(test_pass):
            succeed()
        else:
            fail()

    except:
        e = sys.exc_info()[0]
        print("  >> ERROR :: exception occured !!  [test_101]\n"+
              "  >>            error: " + e)
        fail()


def multiple_connection(conn_count, conn_per_sheet=2, random_sheet_name=False):
    '''
    Fills the global sockets array with tuples in the format (socket object, client ID)
    This function finishes when all threads have connected and finished their handshakes

    Parameters:
        conn_count: How many connections to open
        conn_per_sheet: the number of clients that will be attached to one sheet (2 is a special value for test_13, and makes connections be a 2d list)
        random_sheet_name: If true, then randomize sheet names to request to open. Defaults to false.
    '''
    global connections_lock
    global connections

    # Helper to make a single connection and append it to the global sockets list
    def make_conn(i, ii):
        global connections_lock
        global connections
        temp = connection("user_" + str(i), random_sheet_name, "sheetname_" + str(ii)) if conn_per_sheet != 2 else connection(f"user_{ii}_{i}", False, "sheetname_" + str(ii))
        connections_lock.acquire()
        if conn_per_sheet == 2:
            connections[ii][i] = temp
        else:
            connections.append(temp)
        connections_lock.release()

    # Form multiple connections and add them to the sockets list
    if conn_per_sheet == 2:
        connections = [[() for j in range(2)] for i in range(conn_count//2)]
        threads = []
        for i in range(conn_count//2):
            for j in range(2):
                threads.append(temp := threading.Thread(target=make_conn, args=([j, i])))
                temp.start()
    else:
        threads = []
        for i in range(0,conn_count,conn_per_sheet):
            for j in range(i,i+conn_per_sheet):
                threads.append(temp := threading.Thread(target=make_conn, args=([j, i])))
                temp.start()
            # threads[i].start()
            # time.sleep(0.1)
            # threads[i+1].start()

    # Wait for handshake to finish in each connection
    for t in threads:
        t.join()


def get_random_cell_name():
    """Generate a random cell name"""
    letters = list("ABCDEFGHIJKLMNOPQRSTUVWXYZ")
    letter = random.choice(letters)
    num = str(random.randint(1, 99))
    return letter + num


test_counter:int = 0
test_c_lock = threading.Lock()

def test_14():
    """Stress test - invalid request (bad undo, bad revert, circular dependencies)"""
    global connections
    global sockets_lock

    time_out = 30
    client_count = 24
    tests_per_client = 5
    clients_per_sheet = 4

    global test_counter
    test_counter = 0
    global test_c_lock


    try:
        test_info(time_out, f'Stress test - invalid requests ({clients_per_sheet}x clients/sheet w/ {tests_per_client}x rand tests per {client_count}x clients)')
        timer_thread = threading.Thread(target=time_limit, args=([time_out]))
        timer_thread.start()

        test_pass:dict = {}
        # Connect 30 clients to server
        multiple_connection(client_count, clients_per_sheet)

        # def invalid_undo():
        #     pass

        # def invalid_revert():
        #     pass

        # def invalid_circ_depend():
        #     pass

        def invalid_request(socket_info, rand, t_num):
            """Runs one invalid request from a socket"""
            # print(f"T#: {t_num} ::")
            # Generate a random cell name
            cell = get_random_cell_name()

            invalid_undo = {"requestType": "undo"}
            invalid_revert = {"requestType": "revertCell", "cellName": cell}
            circ_depend = {"requestType": "editCell",
                           "cellName": cell, "contents": "=" + cell + "-10"}

            select_json = {"requestType": "selectCell", "cellName": cell}

            # Choose a random error to use
            request:dict = {}
            if rand == 1:
                test_type = "invalid undo"
                cell = ""
                request = invalid_undo
            if rand == 2:
                test_type = "invalid revert"
                # socket_info[0].sendall(convert_encode(select_json))
                request = invalid_revert
            if rand == 3:
                test_type = "circ depend"
                # socket_info[0].sendall(convert_encode(select_json))
                request = circ_depend
            exp = ["requestError", cell]
            socket_info[0].sendall(convert_encode(request))

            # Wait to receive a valid requestError from server
            data = ""
            while True:
                data += socket_info[0].recv(1024).decode('utf-8')
                messages = parse_json(data)

                for msg in messages:
                    if msg["messageType"] == "requestError" and msg["cellName"] == cell:
                        test_pass[t_num] =  (valid_json(msg, exp),test_type,request,msg,exp,socket_info[2])
                        # test_pass[t_num] = (temp := (valid_json(msg, exp),test_type,request,msg,exp,socket_info[2]))
                        # print(f"Test # {t_num}:")
                        # print(f"     success?: {temp[0]}")
                        # print(f"    test type: {temp[1]}")
                        # print(f"      request: {temp[2]}")
                        # print(f"          msg: {temp[3]}")
                        # print(f"          exp: {temp[4]}")
                        # print(f"     userName: {temp[5]}\n")
                        return

        def run_invalid_req_tests(socket_info):
            """Run multiple tests on a socket"""
            global test_counter
            global test_c_lock
            # ord = [1,2,3]
            # random.shuffle(ord)
            for i in range(tests_per_client):
                test_c_lock.acquire()
                test_counter += 1
                t_num = test_counter
                test_c_lock.release()
                invalid_request(socket_info, random.randint(1,3), t_num)
            socket_info[0].close()

        # Run a thread for each socket
        threads = []
        for i in range(len(connections)):
            threads.append(threading.Thread(
                target=run_invalid_req_tests, args=([connections[i]])))
            threads[i].start()

        # Wait for threads
        for t in threads:
            t.join()

        # print("TEST RESULTS:")
        # for i,line in zip(test_pass.keys(), test_pass.values()):
        #     print(f"Test # {i}:")
        #     print(f"     success?: {line[0]}")
        #     print(f"    test type: {line[1]}")
        #     print(f"      request: {line[2]}")
        #     print(f"          msg: {line[3]}")
        #     print(f"          exp: {line[4]}")
        #     print(f"     userName: {line[5]}\n")
        # Pass if all threads complete
        if len(connections) * tests_per_client == len(test_pass) and all([i[0] for i in test_pass.values()]):
            succeed()
        else:
            fail()

    except:
        e = sys.exc_info()[0]
        print("  >> ERROR :: exception occured !!  [test_101]\n"+
              "  >>            error: " + e)
        fail()


def test_15():
    """Select cell stress test"""

    global connections
    global sockets_lock

    client_count = 30
    time_out = 30
    try:
        test_info(time_out, f'Stress test - select cell ({client_count}x clients, all on one sheet)')
        timer_thread = threading.Thread(target=time_limit, args=([time_out]))
        timer_thread.start()

        # Connect 30 clients to server
        multiple_connection(client_count, client_count)

        def listen_cell_selected(cellName, selector, selectorName, socket):
            """listen for cell selected"""
            data = ""
            while True:
                data += socket.recv(1024).decode('utf-8')
                messages = parse_json(data)

                for message in messages:
                    if valid_json(message, ["cellSelected", cellName, selector, selectorName]):
                        return

        # Each iteration is one client selecting a cell and the other clients listening for it
        for i in range(client_count):
            cell = get_random_cell_name()
            select_json = {"requestType": "selectCell", "cellName": cell}

            # Start actively listening for cellSelected on all other clients including the sender (because the selector client should not update till the server says so)
            threads = []
            for j in range(client_count):
                threads.append(threading.Thread(target=listen_cell_selected,
                                                    args=([cell, connections[i][1], connections[i][2], connections[j][0]]))) # pass in helper
                threads[j].start()

            # Send cell selection
            connections[i][0].sendall(convert_encode(select_json))

            # Wait for receive all of cellSelected on all the other clients
            for thread in threads:
                thread.join()


        succeed()
    except:
        e = sys.exc_info()[0]
        print("  >> ERROR :: exception occured !!  [test_101]\n"+
              "  >>            error: " + e)
        fail()


def test_16():
    """Test 16 - Stress Test for Editing a cell"""

    global connections
    global sockets_lock

    client_count = 24
    time_out = 30

    try:
        test_info(time_out, f'Stress test - Editing a cell ({client_count}x clients all on the same sheet, each client edits once then all confirm the change)')

        multiple_connection(client_count, client_count)

        # listen for cell edits
        def listen_cell_update(socket, cellName, cellContents):
            data = ""
            while True:
                data += socket.recv(1024).decode('utf-8')
                messages = parse_json(data)

                for message in messages:
                    if valid_json(message, ["cellUpdated", cellName, cellContents]):
                        return

        timer_thread = threading.Thread(target=time_limit, args=([time_out]))
        timer_thread.start()

        # Each iteration is one client selecting a cell and the other clients listening for it
        for sending_client in connections:

            cell = get_random_cell_name()
            contents = str(random.randint(-1000, 1000))
            select_json = {"requestType": "selectCell", "cellName": cell}
            edit_json = {"requestType": "editCell",
                         "cellName": cell, "contents": contents}

            # Start actively listening for cellSelected on all other clients
            threads = []
            for j in range(client_count):
                threads.append(threading.Thread(target=listen_cell_update,
                                                    args=([connections[j][0], cell, contents]))) # pass in helper
                threads[j].start()
            # Send cell selection and edit
            # sending_client[0].sendall(convert_encode(select_json))
            sending_client[0].sendall(convert_encode(edit_json))

            # Wait for receive of cellSelected on all the other clients
            for thread in threads:
                thread.join()

        succeed()

    except:
        e = sys.exc_info()[0]
        print("  >> ERROR :: exception occured !!  [test_101]\n"+
              "  >>            error: " + e)
        fail()


# def __NOT_NEEDED__test_17():
#     """Tests requesting edit of cell that doesn't match the selection"""
#     client_count = 2
#     time_out = 30
#     try:
#         test_info(time_out, 'Wrong cell edit test')
#         timer_thread = threading.Thread(target=time_limit, args=([time_out]))
#         timer_thread.start()
#
#         multiple_connection(client_count, client_count)
#
#         select_json = {"requestType": "selectCell", "cellName": "F2"}
#         edit_json = {"requestType": "editCell",
#             "cellName": "H20", "contents": "alskfdj"}
#
#         connections[0][0].sendall(convert_encode(select_json))
#
#         def listen_update(socket):
#             # listen for cell edits
#             data = ""
#             while True:
#                 data += socket.recv(1024).decode('utf-8')
#                 messages = parse_json(data)
#
#                 for message in messages:
#                     if message["messageType"] == "cellUpdated":
#                         fail()
#
#         success_timer_thread = threading.Thread(
#             target=time_limit, args=([10, False]))
#         success_timer_thread.start()
#
#         thread1 = threading.Thread(
#             target=listen_update, args=([connections[0][0]]))
#         thread2 = threading.Thread(
#             target=listen_update, args=([connections[1][0]]))
#
#         thread1.start()
#         thread2.start()
#
#         thread1.join()
#         thread2.join()
#
#     except:
#         fail()


def test_17():
    """New spreadsheet stress test"""

    global HOST
    global PORT
    global connections

    time_out = 20
    client_count = 30

    try:
        test_info(time_out, 'New spreadsheet stress test')
        timer_thread = threading.Thread(target=time_limit, args=([time_out]))
        timer_thread.start()

        connections = []
        multiple_connection(client_count, 1)

        with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
            s.connect((HOST, PORT))

            # Send username for first part of handshake
            s.sendall("bob\n".encode('utf-8'))

            # receive list of files
            data = ""
            while not (len(data) >= 2 and data[-2:] == '\n\n'):
                data += s.recv(1024).decode('utf-8')

            sheets = data.split('\n')[:-2]

            for i in range(client_count):
                if ("sheetname_" + str(i)) not in sheets:
                    fail()

            succeed()

    except:
        fail()


def test_18():
    """Test 18: Test for multiple handshakes occuring at the same time race condition."""

    global HOST
    global PORT

    try:
        time_out = 5
        test_info(time_out, 'Check for race conditions when connecting to the same sheet at the same time. (2x clients)')

        timer_thread = threading.Thread(target=time_limit, args=([time_out]))
        timer_thread.start()

        socket1 = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        socket1.connect((HOST, PORT))

        # Send username for first part of handshake
        socket1.sendall(b'username1\n')

        # receive list of files
        data = ""
        while not (len(data) >= 2 and data[-2:] == '\n\n'):
            data += socket1.recv(1024).decode('utf-8')

        socket2 = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        socket2.connect((HOST, PORT))

        # Send username for first part of handshake
        socket2.sendall(b'username2\n')

        # receive list of files
        data = ""
        while not (len(data) >= 2 and data[-2:] == '\n\n'):
            data += socket2.recv(1024).decode('utf-8')


        socket2.sendall(b'test_18\n')
        data = ""
        while True:
            data += socket2.recv(1024).decode('utf-8')  # socket2 wait for end of handshake
            if re.search("(\d+)\n", data) is not None:
                break

        select_json = {"requestType": "selectCell", "cellName": "C20"}
        edit_json = {"requestType": "editCell",
            "cellName": "C20", "contents": "hello"}


        socket1.sendall(b'test_18\n')   # slowing socket1 down to not complete transaction until after socket 2 finishes.
        # data = ""
        # while True:
        #     data += socket1.recv(1024).decode('utf-8')  # socket1 wait for end of handshake
        #     if re.search("(\d+)\n", data) is not None:
        #         break

        socket2.sendall(convert_encode(select_json))
        socket2.sendall(convert_encode(edit_json))


        data = ""
        while True:
            data += socket1.recv(1024).decode('utf-8')
            messages = parse_json(data)

            for message in messages:
                if valid_json(message, ["cellUpdated", "C20", "hello"]):
                    succeed()

    except:
        fail()





# def __NOT_NEEDED__DUP_OF_BETTER_TESTS__test_20():
#     """Undo + revert test"""
#     try:
#         test_info(20, 'Undo and revert test')
#
#         timer_thread = threading.Thread(target=time_limit, args=([20]))
#         timer_thread.start()
#
#         multiple_connection(2)
#
#         select_json = {"requestType": "selectCell", "cellName": "B1"}
#         edit_json = {"requestType": "editCell",
#             "cellName": "B1", "contents": "hello"}
#         edit_json2 = {"requestType": "editCell",
#             "cellName": "B1", "contents": "bye"}
#
#         revert_json = {"requestType": "revertCell", "cellName": "B1"}
#         undo_json = {"requestType": "undo"}
#
#         for message in (select_json, edit_json, edit_json2):
#             connections[0][0].sendall(convert_encode(message))
#
#         def listen_revert(socket):
#             data = ""
#             while True:
#                 data += socket.recv(1024).decode('utf-8')
#                 messages = parse_json(data)
#
#                 for message in messages:
#                     if valid_json(message, ["cellUpdated", "B1", "hello"]):
#                         return
#
#         thread1 = threading.Thread(
#             target=listen_revert, args=([connections[0][0]]))
#         thread2 = threading.Thread(
#             target=listen_revert, args=([connections[1][0]]))
#
#         thread1.start()
#         thread2.start()
#
#         connections[0][0].sendall(convert_encode(revert_json))
#
#         thread1.join()
#         thread2.join()
#
#         def listen_undo(socket):
#             data = ""
#             while True:
#                 data += socket.recv(1024).decode('utf-8')
#                 messages = parse_json(data)
#
#                 for message in messages:
#                     if valid_json(message, ["cellUpdated", "B1", "bye"]):
#                         return
#
#         thread1 = threading.Thread(target=listen_undo, args=([connections[0][0]]))
#         thread2 = threading.Thread(target=listen_undo, args=([connections[1][0]]))
#
#         thread1.start()
#         thread2.start()
#
#         connections[1][0].sendall(convert_encode(undo_json))
#
#         thread1.join()
#         thread2.join()
#
#         succeed()
#
#     except:
#         fail()


def main(argv):
    argc = len(argv)  # getting command line args length
    global PORT
    global HOST
    global sockets_lock

    sockets_lock = threading.Lock()

    if argc == 1:
        print(total_test_num)  # print total test numbers
    elif argc == 3:
        test_num = int(argv[1])
        if ':' in argv[2]:
            temp = argv[2].split(':')
            HOST = temp[0]
            PORT = int(temp[1])
        else:
            PORT = int(argv[2])
        if test_num > total_test_num:
            print("no such test exists")
        else:
            eval("test_" + str(test_num) + "()")
    else:
        print("pass 0 or 2 arguments")
    print("DEBUG :: end of test")


if __name__ == "__main__":
    main(sys.argv)
