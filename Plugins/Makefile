CMAKE ?= cmake
COPY ?= cp
# WebGL/Emscripten setup
EMCMAKE ?= emcmake
# macOS code signing setup
CODESIGN ?= codesign
MACOS_CODESIGN_SIGNATURE ?=
# Android setup
ANDROID_NDK_ROOT ?=

CMAKE_ARGS += \
	-DCMAKE_MAKE_PROGRAM="$(MAKE)" \
	-DLOTTIE_MODULE=OFF
ifeq ($(DEBUG),1)
	CMAKE_ARGS += -DCMAKE_BUILD_TYPE=Debug
else
	CMAKE_ARGS += -DCMAKE_BUILD_TYPE=Release
endif

BUILD_DIRS = \
	lib/Windows/x86_64 lib/Windows/x86 \
	lib/Linux/x86_64 \
	lib/macOS lib/iOS lib/tvOS lib/visionOS \
	lib/Android/arm64 lib/Android/arm32 lib/Android/x86 lib/Android/x86_64 \
	lib/WebGL

# Misc
$(BUILD_DIRS):
	mkdir -p $@

lib/%/build~/librlottie.dll: rlottie~ cmake-toolchains~/%.cmake
	$(CMAKE) rlottie~ -B $(@D) --toolchain $(abspath cmake-toolchains~/$*.cmake) $(CMAKE_ARGS) -DBUILD_SHARED_LIBS=ON
	$(CMAKE) --build $(@D)

lib/%/build~/librlottie.so: rlottie~ cmake-toolchains~/%.cmake
	$(CMAKE) rlottie~ -B $(@D) --toolchain $(abspath cmake-toolchains~/$*.cmake) $(CMAKE_ARGS) -DBUILD_SHARED_LIBS=ON
	$(CMAKE) --build $(@D)

lib/%/build~/librlottie.dylib: rlottie~ cmake-toolchains~/%.cmake
	$(CMAKE) rlottie~ -B $(@D) --toolchain $(abspath cmake-toolchains~/$*.cmake) $(CMAKE_ARGS) -DBUILD_SHARED_LIBS=ON
	$(CMAKE) --build $(@D)
ifdef MACOS_CODESIGN_SIGNATURE
	$(CODESIGN) -s "$(MACOS_CODESIGN_SIGNATURE)" $@
endif

lib/%/build~/librlottie.a: rlottie~ cmake-toolchains~/%.cmake
	$(CMAKE) rlottie~ -B $(@D) --toolchain $(abspath cmake-toolchains~/$*.cmake) $(CMAKE_ARGS) -DBUILD_SHARED_LIBS=OFF
	$(CMAKE) --build $(@D)

lib/WebGL/build~/librlottie.a: CMAKE_ARGS += -DLOTTIE_THREAD=OFF
lib/WebGL/build~/librlottie.a: rlottie~
	$(EMCMAKE) cmake rlottie~ -B $(@D) $(CMAKE_ARGS) -DBUILD_SHARED_LIBS=OFF
	$(CMAKE) --build $(@D)

lib/%/lottie-player.dll: lib/%/build~/librlottie.dll
	$(COPY) $< $@

lib/%/liblottie-player.so: lib/%/build~/librlottie.so
	$(COPY) $< $@

lib/%/liblottie-player.dylib: lib/%/build~/librlottie.dylib
	$(COPY) $< $@

lib/%/liblottie-player.a: lib/%/build~/librlottie.a
	$(COPY) $< $@


# Android
check-ndk-root:
ifndef ANDROID_NDK_ROOT
	$(error ANDROID_NDK_ROOT must be set for Android builds!)
endif
ifeq ($(wildcard $(ANDROID_NDK_ROOT)/toolchains/llvm/prebuilt/*/bin),)
	$(error ANDROID_NDK_ROOT must point to a valid NDK installation. Could not find "$(ANDROID_NDK_ROOT)/toolchains/llvm/prebuilt/*/bin")
endif

lib/Android/arm64/liblottie-player.so: check-ndk-root
lib/Android/arm32/liblottie-player.so: check-ndk-root
lib/Android/x86_64/liblottie-player.so: check-ndk-root
lib/Android/x86/liblottie-player.so: check-ndk-root

# Targets
windows-x86_64: lib/Windows/x86_64/lottie-player.dll
windows-x86: lib/Windows/x86/lottie-player.dll
all-windows: windows-x86_64 windows-x86

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

webgl: lib/WebGL/liblottie-player.a
all-webgl: webgl