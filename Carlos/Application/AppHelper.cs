using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
namespace Carlos.Application
{
   /// <summary>
   /// 应用程序帮助类，实现一些基础的应用程序或者程序集的实用代码。
   /// </summary>
   public sealed class AppHelper
   {
#if WIN32
      /// <summary>
      /// 获取一个值，该值指示提供的路径中的程序集清单是否包含强名称签名。
      /// </summary>
      /// <param name="wszFilePath">需要被验证的应用程序的路径。</param>
      /// <param name="fForceVerification">是否需要重写注册表设置。</param>
      /// <param name="pfWasVerified">如果验证了强名称签名，则为true；否则为false。如果由于注册表设置验证成功，pfwasverized也会设置为false。</param>
      /// <returns>该操作将会返回一个Boolean值，这个值如果为true则表示需要验证的程序有强名称签名，否则为没有。</returns>
      [DllImport("mscoree.dll", CharSet = CharSet.Unicode)]
      private static extern bool StrongNameSignatureVerificationEx(string wszFilePath, bool fForceVerification, ref bool pfWasVerified);
      /// <summary>
      /// 验证程序是否有强名称签名。
      /// </summary>
      /// <param name="assemblyFilePath">需要被验证的应用程序路径。</param>
      /// <returns>该操作将会返回一个Boolean值，这个值如果为true则表示需要验证的程序有强名称签名，否则为没有。</returns>
      public static bool HasStrongNameSignature(string assemblyFilePath)
      {
         bool wasVerified = false;
         return StrongNameSignatureVerificationEx(assemblyFilePath, false, ref wasVerified);
      }
#endif
      /// <summary>
      /// 根据程序集路径获取符合CLR规范的程序集实例。
      /// </summary>
      /// <param name="assemblyFilePath">符合CLR规范的程序集的路径。</param>
      /// <returns>该操作将会返回一个符合CLR规范的程序集的实例。</returns>
      /// <exception cref="FileNotFoundException">当找不到指定的程序集文件路径时，则将会抛出这个异常。</exception>
      /// <exception cref="BadImageFormatException">当指定路径的程序集不符合CLR规范时，则将抛出这个异常。</exception>
      public static Assembly GetCLSAssemblyInstance(string assemblyFilePath)
      {
         Assembly assembly;
         if (!File.Exists(assemblyFilePath)) throw new FileNotFoundException("Not found this assembly file.", assemblyFilePath);
         try
         {
            assembly = Assembly.LoadFrom(assemblyFilePath);
            return assembly;
         }
         catch (BadImageFormatException throwedException)
         {
            if (throwedException != null)
            {
               throwedException = new BadImageFormatException("Only support CLR assembly.", assemblyFilePath);
               throw throwedException;
            }
            return null;
         }
      }
      /// <summary>
      /// 获取指定的程序集的基本的应用程序信息。
      /// </summary>
      /// <param name="assembly">需要被获取基础应用程序信息的程序集。</param>
      /// <returns>该操作将会返回一个CLSAssemblyInfo结构实例，这个实例包含了一个应用程序最基本的信息，比如说名称，完全限定名称，版本号，版权信息等等。</returns>
      public static CLSAssemblyInfo GetCLSAssemblyInfo(Assembly assembly)
      {
         CLSAssemblyInfo assemblyInfo = new CLSAssemblyInfo()
         {
            Name = assembly.GetName().Name,
            FileVersion = new Version(FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion),
            ProductVersion = new Version(FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion),
            Copyright = ((AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyCopyrightAttribute))).Copyright,
            FullName = assembly.GetName().FullName
         };
         return assemblyInfo;
      }
   }
}
