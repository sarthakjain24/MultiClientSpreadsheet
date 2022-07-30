#* @author Andrew Osterhout (osterhoutan)
#* A hopefully workign and versitle make file so we don't really have to do any work with it
#* Modifed (heavily) from: https://riptutorial.com/makefile/example/21376/building-from-different-source-folders-to-different-target-folders

# Set project directory one level above of Makefile directory. $(CURDIR) is a GNU make variable containing the path to the current working directory
PROJDIR = .
# PROJDIR = $(realpath $(CURDIR))
SOURCEDIR = $(PROJDIR)/src
BUILDDIR = $(PROJDIR)/build/bin
LOCLIBDIR = $(PROJDIR)/lib
TESTSRCDIR = $(PROJDIR)/test
TESTBUILDDIR = $(PROJDIR)/build/test

#<< Name of the final executable
TARGETEXE := spreadsheet_server

TARGET := $(BUILDDIR)/$(TARGETEXE)


#<<  Define directories for project to store data in
#? (see `REL_LOC` for settings on the relitive location to the exicutible these should have)
DATADIRS := $(PROJDIR)/spreadsheets

#<<  Define directories that resources the project will be dependent on will be stored 
#? (see `REL_LOC` for settings on how it wil be moved around for )
RESDIRS := 

#<< configure external lib (absolute) paths
EXTLIBS := 

# Decide whether the commands will be shwon or not (comment out to pass via command line)
VERBOSE := TRUE

# OS specific part
ifeq ($(OS),Windows_NT)
	RM = del /F /Q 
	RMDIR = -RMDIR /S /Q
	MKDIR = -mkdir
	CP = xcopy /f
	ERRIGNORE = 2>NUL || true
	SEP=\\
else
	RM = rm -rf 
	RMDIR = rm -rf 
	MKDIR = mkdir -p
	CP = cp -rf
	ERRIGNORE = 2>/dev/null
	SEP=/
endif

# Reconfigure build if degug variable set to true
DEBUG = FALSE
ifeq ($(DEBUG),TRUE)
	override DEBUG := -g -D DEBUG
	BUILDDIR := $(PROJDIR)/build/debug
	TARGET := $(BUILDDIR)/$(TARGETEXE)
	REMOVE_PREV_TEST_DATA := $(RM) ./spreadsheets/*.ss_json $(ERRIGNORE)
	SET_TEST_DATA := $(CP) ./test/data/spreadsheets/*.ss_json ./spreadsheets/ $(ERRIGNORE)
else
	override DEBUG := 
	SET_TEST_DATA := printf "Doing nothing this feture is currently diabled w/out DEBUG defined to TRUE\n"
endif

# fancy color coded done text for printf output
FDONE = \e[1;32mDONE\e[0m

# Create the list of directories
SOURCEDIRS = $(shell find $(SOURCEDIR) -type d)
LOCLIBDIRS = $(shell find $(LOCLIBDIR) -type d)
# TESTSRCDIRS = $(shell find $(TESTSRCDIR) -type d)
TARGETDIRS = $(patsubst $(SOURCEDIR)%,$(BUILDDIR)%,$(SOURCEDIRS))
# TARGETDIRS += $(patsubst $(TESTSRCDIR)%,$(TESTBUILDDIR)%,$(TESTSRCDIRS))
# DIRS = #list of dirs in src structure
# SOURCEDIRS = $(foreach dir, $(DIRS), $(addprefix $(SOURCEDIR)/, $(dir)))
# TARGETDIRS = $(foreach dir, $(DIRS), $(addprefix $(BUILDDIR)/, $(dir)))

# Generate the GCC includes parameters by adding -I before each source folder
INCLUDES = $(foreach dir, $(SOURCEDIRS), $(addprefix -I, $(dir)))
INCLUDES += $(foreach dir, $(LOCLIBDIRS), $(addprefix -I, $(dir)))
# INCLUDES += $(foreach dir, $(EXTLIBS), $(addprefix -I, $(dir)))

# Add this list to VPATH, the place make will look for the source files
VPATH = $(SOURCEDIRS)

# Create a list of *.cpp sources in DIRS
SOURCES = $(foreach dir,$(SOURCEDIRS),$(wildcard $(dir)/*.cpp))
# TESTSOURCES = $(foreach dir,$(TESTSRCDIRS),$(wildcard $(dir)/*.cpp))

# Define objects for all sources
OBJS = $(subst $(SOURCEDIR),$(BUILDDIR),$(SOURCES:.cpp=.o))

# Define dependencies files for all objects
DEPS = $(OBJS:.o=.d)

# Name the compiler
CC = g++
# Set compiler flags that should always be present
CFLAGS = -std=c++17
# Set flags that the linker will need
LFLAGS = -lboost_system -lboost_thread -lpthread

# Remove space after separator
PSEP = $(strip $(SEP))

# Hide or not the calls depending of VERBOSE
ifeq ($(VERBOSE),TRUE)
	HIDE =  
else
	HIDE = @
endif

# Define the function that will generate each rule
define generateRules
$(1)/%.o: %.cpp %.hpp
	@printf "Building $$@ ...\n"
	$$(HIDE)$$(CC) $$(CFLAGS) $$(DEBUG) -c $$(INCLUDES) -o $$(subst /,$$(PSEP),$$@) $$(subst /,$$(PSEP),$$<)
	@printf "$$(FDONE) Building: $$@\n"
endef

.PHONY: all clean directories 

all: dataDirs test_data $(TARGET)
	@printf "!! the 'all' task is $(FDONE) !!\n"

$(TARGET): directories $(OBJS)
	@printf "Linking $@\n"
	$(HIDE)$(CC) $(CFLAGS) $(DEBUG) $(INCLUDES) $(OBJS) $(LFLAGS) -o $(TARGET)
	@printf "!! BUILD of $@ $(FDONE) !!\n"

# Include dependencies
# -include $(DEPS)

# Generate rules
$(foreach targetdir, $(TARGETDIRS), $(eval $(call generateRules, $(targetdir))))

directories: 
	@printf "Making build directories...\n"
	$(HIDE)$(MKDIR) $(subst /,$(PSEP),$(TARGETDIRS)) $(ERRIGNORE)
	@printf "$(FDONE) Build directories made.\n"

dataDirs:
	@printf "Making the project Data (directories the project needs for stpring data)\n"
	$(HIDE)$(MKDIR) $(subst /,$(PSEP),$(DATADIRS)) $(ERRIGNORE)
	@printf "$(FDONE) Extra but nessisary directories made.\n"

test_directories:
	@printf "Making test beuild directories...\n"
	$(HIDE)$(MKDIR) $(TESTBUILDDIR) $(ERRIGNORE)
	$(HIDE)$(MKDIR) $(TESTBUILDDIR)/spreadsheets $(ERRIGNORE)
	@printf "$(FDONE) Test build directories made.\n"

test_data:
	@printf "Removing data from previouse tests & replacing with starting data\n"
	$(HIDE)$(SET_TEST_DATA)
	@printf "$(FDONE) Resetting Data for Testing Complete.\n"


# Remove all objects, dependencies and executable files generated during the build
clean:
	$(HIDE)$(RMDIR) $(subst /,$(PSEP),$(TARGETDIRS)) $(ERRIGNORE)
	$(HIDE)$(RMDIR) $(subst /,$(PSEP),$(TESTBUILDDIR)) $(ERRIGNORE)
	$(HIDE)$(RMDIR) $(subst /,$(PSEP),$(PROJDIR)/build/debug) $(ERRIGNORE)
	$(HIDE)$(RMDIR) $(subst /,$(PSEP),$(PROJDIR)/build/bin) $(ERRIGNORE)
	$(HIDE)$(RM) $(TARGET) $(ERRIGNORE)
	@printf "Cleaning $(FDONE) !!\n"


# directive for checking if the make file is properly set up
testme:
	@printf "!! THIS IS A TEST OF THE MAKE FILE !! \n"
	@printf "cwd :: $(CURDIR) \n"
	@printf "target :: $(TARGET) \n"
	@printf "projDir :: $(PROJDIR) \n"
	@printf "src Dir :: $(SOURCEDIR) \n"
	@printf "build Dir :: $(BUILDDIR) \n"
	@printf "local lib Dir :: $(LOCLIBDIR) \n"
	@printf "source dirs :: $(SOURCEDIRS) \n"
	@printf "build dirs :: $(TARGETDIRS) \n"
	@printf "local lib dirs :: $(LOCLIBDIRS) \n"
	@printf "source files :: $(SOURCES) \n"
	@printf "forced includes :: $(INCLUDES) \n"
	@printf "objects :: $(OBJS) \n"
	@printf "DEBUG value :: $(DEBUG) \n"
	@printf "$(FDONE) :: !! END OF TEST !! \n"


# ============================================================================ #
#                                 CUSTOM TESTS                                 #
# ============================================================================ #
#? TO BUILD a test just tye `make test TEST=` then the name of the cpp test code in the ./test dir (without file extension) 

test: $(TESTBUILDDIR)/$(TEST) directories test_directories
	@printf "$(FDONE) :: $(TEST) made, and ready to run !!"

$(TESTBUILDDIR)/$(TEST): directories test_directories $(OBJS) $(TARGET) $(TESTSRCDIR)/$(TEST).cpp
	@printf Building Test $@
	$(HIDE)$(CC) $(CFLAGS) $(DEBUG) -c $(INCLUDES) -o $(TESTBUILDDIR)/$(TEST).o $(TESTSRCDIR)/$(TEST).cpp
	@printf Linking $@
	$(HIDE)$(CC) $(CFLAGS) $(DEBUG) $(INCLUDES) $(filter-out $(BUILDDIR)/main.o,$(OBJS)) $(TESTBUILDDIR)/$(TEST).o $(LFLAGS) -o $(TARGET)
	@printf "$(FDONE) Building test: $@ !!"
