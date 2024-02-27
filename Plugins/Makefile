AR ?= ar
CMAKE ?= cmake
EMCXX ?= em++
STRIP ?= strip

ANDROID_NDK_ROOT ?=

LINKFLAGS += -shared
RLOTTIE_CXXFLAGS = -std=c++14 \
	-fno-exceptions \
	-fno-unwind-tables \
	-fno-asynchronous-unwind-tables \
	-fno-rtti \
	-Wall \
	-fvisibility=hidden \
	-Wnon-virtual-dtor \
	-Woverloaded-virtual \
	-Wno-unused-parameter
CXXFLAGS += $(RLOTTIE_CXXFLAGS) -Isrc~ -Irlottie~/inc -Irlottie~/src -Irlottie~/src/vector -Irlottie~/src/vector/freetype
ifeq ($(DEBUG),1)
	CXXFLAGS += -O0 -g
else
	CXXFLAGS += -O2
endif

BUILD_DIRS = \
	build/Windows/x86_64 build/Windows/x86 \
	build/Linux/x86_64 \
	build/macOS build/iOS build/tvOS build/visionOS \
	build/Android/arm64 build/Android/arm32 build/Android/x86 build/Android/x86_64 \
	build/WebGL

SRC = $(wildcard src~/*.cpp src~/*.h)
RLOTTIE_SRC = $(filter-out %/wasm/% %/stb/%,$(wildcard rlottie~/src/**/*.cpp))

# Misc
$(BUILD_DIRS):
	mkdir -p $@

%/lottie-player.dll: $(SRC) $(RLOTTIE_SRC) | %
	$(CXX) -o $@ $(filter src~/%.cpp, $^) $(CXXFLAGS) $(LINKFLAGS)
	$(STRIP) -x $@

%/liblottie-player.so: CXXFLAGS += -fPIC
%/liblottie-player.so: $(SRC) $(RLOTTIE_SRC) | %
	$(CXX) -o $@ $(filter src~/%.cpp, $^) $(CXXFLAGS) $(LINKFLAGS)
	$(STRIP) -x $@

%/liblottie-player.dylib: $(SRC) $(RLOTTIE_SRC) | %
	$(CXX) -o $@ $(filter src~/%.cpp, $^) $(CXXFLAGS) $(LINKFLAGS)
	$(STRIP) -x $@

%/lottie-player.o: $(SRC) $(RLOTTIE_SRC) | %
	$(CXX) -c -o $@ $(filter src~/%.cpp, $^) $(CXXFLAGS)

%/liblottie-player.a: %/lottie-player.o | %
	$(AR) r $@ $<
	$(STRIP) -x $@

# Linux
build/Linux/x86/liblottie-player.so: CXXFLAGS += -m32

# macOS
build/macOS/liblottie-player.dylib: CXXFLAGS += -arch x86_64 -arch arm64

# iOS
build/iOS/liblottie-player.a: CXXFLAGS += -arch arm64 -isysroot $(shell xcrun --show-sdk-path --sdk iphoneos)

# tvOS
build/tvOS/liblottie-player.a: CXXFLAGS += -arch arm64 -isysroot $(shell xcrun --show-sdk-path --sdk appletvos)

# visionOS
build/visionOS/liblottie-player.a: CXXFLAGS += -arch arm64 -isysroot $(shell xcrun --show-sdk-path --sdk xros)

# Android
build/Android/%/liblottie-player.so: CXXFLAGS += -static-libstdc++
build/Android/%/liblottie-player.so: STRIP = $(wildcard $(ANDROID_NDK_ROOT)/toolchains/llvm/prebuilt/*/bin/llvm-strip)

build/Android/arm64/liblottie-player.so: CXX = $(wildcard $(ANDROID_NDK_ROOT)/toolchains/llvm/prebuilt/*/bin/aarch64-linux-android21-clang++)
build/Android/arm32/liblottie-player.so: CXX = $(wildcard $(ANDROID_NDK_ROOT)/toolchains/llvm/prebuilt/*/bin/armv7a-linux-androideabi19-clang++)
build/Android/x86_64/liblottie-player.so: CXX = $(wildcard $(ANDROID_NDK_ROOT)/toolchains/llvm/prebuilt/*/bin/x86_64-linux-android21-clang++)
build/Android/x86/liblottie-player.so: CXX = $(wildcard $(ANDROID_NDK_ROOT)/toolchains/llvm/prebuilt/*/bin/i686-linux-android19-clang++)

# WebGL
build/WebGL/lottie-player.bc: src~/lottie-player.cpp | build/WebGL
	$(EMCXX) -o $@ -c $< $(CXXFLAGS) -emit-llvm

# Targets
windows-x86_64: build/Windows/x86_64/lottie-player.dll
windows-x86: build/Windows/x86/lottie-player.dll
all-windows: windows-x86_64 windows-x86

windows-mingw-x86_64: CXX = x86_64-w64-mingw32-c++
windows-mingw-x86_64: STRIP = x86_64-w64-mingw32-strip
windows-mingw-x86_64: LINKFLAGS += -static-libgcc -Wl,-Bstatic -lstdc++ -lpthread -Wl,-Bdynamic
windows-mingw-x86_64: build/Windows/x86_64/lottie-player.dll

windows-mingw-x86: CXX = i686-w64-mingw32-c++
windows-mingw-x86: STRIP = i686-w64-mingw32-strip
windows-mingw-x86: LINKFLAGS += -static-libgcc -Wl,-Bstatic -lstdc++ -lpthread -Wl,-Bdynamic
windows-mingw-x86: build/Windows/x86/lottie-player.dll
all-windows-mingw: windows-mingw-x86_64 windows-mingw-x86

linux-x86_64: build/Linux/x86_64/liblottie-player.so
all-linux: linux-x86_64

macos-universal: build/macOS/liblottie-player.dylib
ios-arm64: build/iOS/liblottie-player.a
tvos-arm64: build/tvOS/liblottie-player.a
visionos-arm64: build/visionOS/liblottie-player.a
all-apple: macos-universal ios-arm64 tvos-arm64 visionos-arm64

android-arm64: build/Android/arm64/liblottie-player.so
android-arm32: build/Android/arm32/liblottie-player.so
android-x86_64: build/Android/x86_64/liblottie-player.so
android-x86: build/Android/x86/liblottie-player.so
all-android: android-arm64 android-arm32 android-x86_64 android-x86

webgl: build/WebGL/lottie-player.bc
all-webgl: webgl