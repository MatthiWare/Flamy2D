echo OFF
echo "Compiling OSX"
clang -arch x86_64 -arch i386 -shared -o ./OSX/stb_vorbis.dylib src/stb_vorbis.c

echo "Compiling Win64"
x86_64-w64-mingw32-gcc -shared -o ./Windows/stb_vorbis-win64.dll src/stb_vorbis.c

echo "Compiling Win32"
i686-w64-mingw32-gcc -shared -o ./Windows/stb_vorbis-win32.dll src/stb_vorbis.c

echo "Compiling Linux64"
x86_64-w64-mingw32-gcc -shared -fPIC -o ./Linux/stb_vorbis-i686.so src/stb_vorbis.c

echo "Compiling Linux32"
i686-w64-mingw32-gcc -shared -m32 -fPIC -o ./Linux/stb_vorbis-x86_64.so src/stb_vorbis.c

echo "Done Compiling"
pause