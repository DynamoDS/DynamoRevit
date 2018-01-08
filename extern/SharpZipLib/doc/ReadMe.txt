#ZipLib 
  
Version 0.85.5 

Introduction 
 
#ZipLib is a Zip, GZip, Tar and BZip2 library written entirely in C# for the .NET framework. It is implemented as an assembly (installable in the GAC), and can easily be incorporated into other projects using any .NET language.
#ZipLib was ported from the GNU Classpath ZIP library for use with #Develop (http://www.icsharpcode.net/OpenSource/SD) which needed gzip/zip compression.  Later bzip2 compression and tar archiving was added due to popular demand. 
 
There is a web site from which you can download the assembly and or the source code (http://icsharpcode.net/OpenSource/SharpZipLib).  A forum is also available at http://community.sharpdevelop.net/forums/12/ShowForum.aspx.

License 
The software is released under the GPL with an exception which allows linking with non GPL programs. The exception to the GPL is as follows: 

Linking this library statically or dynamically with other modules is making a combined work based on this library. Thus, the terms and conditions of the GNU General Public License cover the whole combination. 

As a special exception, the copyright holders of this library give you permission to link this library with independent modules to produce an executable, regardless of the license terms of these independent modules, and to copy and distribute the resulting executable under terms of your choice, provided that you also meet, for each linked independent module, the terms and conditions of the license of that module. An independent module is a module which is not derived from or based on this library. 

If you modify this library, you may extend this exception to your version of the library, but you are not obligated to do so. If you do not wish to do so, delete this exception statement from your version. 

  
Building the library 

Currently there are two ways to build this library : 

NAnt (http://nant.sourceforge.net) 
This is a free makefile replacement, I encourage the use of this free build automation utility. Just run the SharpZiplib.build in the src directory. (see the nant documentation for more information about nant). Version 0.85 was used during development. 
SharpDevelop (http://www.icsharpcode.net/OpenSource/SD) 
This is a free IDE for .NET, the projects are available in the sourece and samples download.  Give it a try. 

 
Namespace Layout 
 
Zip implementation ICSharpCode.SharpZipLib.Zip Tar implementation ICSharpCode.SharpZipLib.Tar.* Gzip implementation ICSharpCode.SharpZipLib.GZip.* Bzip2 implementation ICSharpCode.SharpZipLib.BZip2.* Inflater/Deflater streams ICSharpCode.SharpZipLib.Zip.Compression.Streams Core utilities / interfaces ICSharCode.SharpZipLib.Core   
  

Reporting Bugs/Submit Patches 

If you want to submit a patch write to mike@icsharpcode.net. If it is a bug fix then it is required that you write a unit test demonstrating the bug. If you find a bug send me a case (generally a file that fails) or preferably a unit test demonstrating the bug. I'll try to fix it. 

Credits 

#ZipLib was originally developed by Mike Krueger (mike@icsharpcode.net). However, much existing Java code helped a lot in speeding the creation of this library. 
  
Therefore credits fly out to others. 

Zip/Gzip implementation : 
A Java version of the zlib which was originally created by the Free Software Foundation (FSF). So all credits should go to the FSF and the authors who have worked on this piece of code. 

Without the zlib authors the Java zlib wouldn't be possible : 
Jean-loup Gailly(jloup@gzip.org) 
Mark Adler(madler@alumni.caltech.edu) and contributors of zlib. 

For the bzip2 implementation : 
Julian R Seward 

The Java port was done by Keiron Liddle, Aftex Software(keiron@aftexsw.com) 

Credits for the tar implementation fly out to : 
Timothy Gerard Endres(time@gjt.org) 

Special thanks to Christoph Wille for beta testing, suggestions and the setup of the Web site. 

