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
	lib/Windows/x86_64 lib/Windows/x86 \
	lib/Linux/x86_64 \
	lib/macOS lib/iOS lib/tvOS lib/visionOS \
	lib/Android/arm64 lib/Android/arm32 lib/Android/x86 lib/Android/x86_64 \
	lib/WebGL

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
lib/Linux/x86/liblottie-player.so: CXXFLAGS += -m32

# macOS
lib/macOS/liblottie-player.dylib: CXXFLAGS += -arch x86_64 -arch arm64

# iOS
lib/iOS/liblottie-player.a: CXXFLAGS += -arch arm64 -isysroot $(shell xcrun --show-sdk-path --sdk iphoneos)

# tvOS
lib/tvOS/liblottie-player.a: CXXFLAGS += -arch arm64 -isysroot $(shell xcrun --show-sdk-path --sdk appletvos)

# visionOS
lib/visionOS/liblottie-player.a: CXXFLAGS += -arch arm64 -isysroot $(shell xcrun --show-sdk-path --sdk xros)

# Android
lib/Android/%/liblottie-player.so: CXXFLAGS += -static-libstdc++
lib/Android/%/liblottie-player.so: STRIP = $(wildcard $(ANDROID_NDK_ROOT)/toolchains/llvm/prebuilt/*/bin/llvm-strip)

lib/Android/arm64/liblottie-player.so: CXX = $(wildcard $(ANDROID_NDK_ROOT)/toolchains/llvm/prebuilt/*/bin/aarch64-linux-android21-clang++)
lib/Android/arm32/liblottie-player.so: CXX = $(wildcard $(ANDROID_NDK_ROOT)/toolchains/llvm/prebuilt/*/bin/armv7a-linux-androideabi19-clang++)
lib/Android/x86_64/liblottie-player.so: CXX = $(wildcard $(ANDROID_NDK_ROOT)/toolchains/llvm/prebuilt/*/bin/x86_64-linux-android21-clang++)
lib/Android/x86/liblottie-player.so: CXX = $(wildcard $(ANDROID_NDK_ROOT)/toolchains/llvm/prebuilt/*/bin/i686-linux-android19-clang++)

# WebGL
lib/WebGL/lottie-player.bc: src~/lottie-player.cpp | lib/WebGL
	$(EMCXX) -o $@ -c $< $(CXXFLAGS) -emit-llvm

# Targets
windows-x86_64: lib/Windows/x86_64/lottie-player.dll
windows-x86: lib/Windows/x86/lottie-player.dll
all-windows: windows-x86_64 windows-x86

windows-mingw-x86_64: CXX = x86_64-w64-mingw32-c++
windows-mingw-x86_64: STRIP = x86_64-w64-mingw32-strip
windows-mingw-x86_64: LINKFLAGS += -static-libgcc -Wl,-Bstatic -lstdc++ -lpthread -Wl,-Bdynamic
windows-mingw-x86_64: lib/Windows/x86_64/lottie-player.dll

windows-mingw-x86: CXX = i686-w64-mingw32-c++
windows-mingw-x86: STRIP = i686-w64-mingw32-strip
windows-mingw-x86: LINKFLAGS += -static-libgcc -Wl,-Bstatic -lstdc++ -lpthread -Wl,-Bdynamic
windows-mingw-x86: lib/Windows/x86/lottie-player.dll
all-windows-mingw: windows-mingw-x86_64 windows-mingw-x86

linux-x86_64: lib/Linux/x86_64/liblottie-player.so
all-linux: linux-x86_64

macos-universal: lib/macOS/liblottie-player.dylib
ios-arm64: lib/iOS/liblottie-player.a
tvos-arm64: lib/tvOS/liblottie-player.a
visionos-arm64: lib/visionOS/liblottie-player.a
all-apple: macos-universal ios-arm64 tvos-arm64 visionos-arm64

android-arm64: lib/Android/arm64/liblottie-player.so
android-arm32: lib/Android/arm32/liblottie-player.so
android-x86_64: lib/Android/x86_64/liblottie-player.so
android-x86: lib/Android/x86/liblottie-player.so
all-android: android-arm64 android-arm32 android-x86_64 android-x86

webgl: lib/WebGL/lottie-player.bc
all-webgl: webgl