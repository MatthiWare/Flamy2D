echo OFF
echo "Compiling OSX"
clang -arch x86_64 -arch i386 -shared -o ./OSX/stb_image.dylib src/stb_image.c

echo "Compiling Win64"
x86_64-w64-mingw32-gcc -shared -o ./Windows/stb_image-win64.dll src/stb_image.c

echo "Compiling Win32"
i686-w64-mingw32-gcc -shared -o ./Windows/stb_image-win32.dll src/stb_image.c

echo "Compiling Linux64"
x86_64-w64-mingw32-gcc -shared -fPIC -o ./Linux/stb_image-i686.so src/stb_image.c

echo "Compiling Linux32"
i686-w64-mingw32-gcc -shared -m32 -fPIC -o ./Linux/stb_image-x86_64.so src/stb_image.c

echo "Done Compiling"
pause