set(CMAKE_SYSTEM_NAME "iOS")
set(CMAKE_OSX_ARCHITECTURES "arm64")

set(CMAKE_C_COMPILER clang)
set(CMAKE_CXX_COMPILER clang++)
set(CMAKE_SHARED_LINKER_FLAGS_RELEASE "-Wl,-S -Wl,-x")
