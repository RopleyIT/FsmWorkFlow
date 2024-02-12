using System.Reflection;

namespace DynamicAssemblies;

public static class ClassLoader
{
    /// <summary>
    /// Given a path to an assembly and the name of a concrete
    /// class within that assembly, create an instance of the
    /// concrete class and return it as the interface type
    /// specified in the generic parameter.
    /// </summary>
    /// <typeparam name="I">The interface to return</typeparam>
    /// <param name="path">The path to the assembly file</param>
    /// <param name="className">The concrete class that
    /// implements the interface</param>
    /// <returns>A reference to the interface on the
    /// concrete class instantiated from the loaded
    /// assembly</returns>
    /// <exception cref="ArgumentException"></exception>
    
    public static I? GetInterface<I>
        (string path, string className) 
    {
        Type interfaceType = typeof(I);
        string? interfaceName = interfaceType.FullName;
        if (interfaceName == null)
            throw new ArgumentException
                ("Interface type is not implementable");

        // Validation of parameters. 'I' must be a public interface

        if(!interfaceType.IsInterface || !interfaceType.IsPublic)
            throw new ArgumentException
                ($"Type {interfaceName} is not a public interface");

        // The path argument must point to an existent assembly DLL

        if(!File.Exists(path))
            throw new ArgumentException
                ($"Library {path} does not exist");

        // If the file is not a valid assembly, the following
        // call to Load() will throw an exception

        Assembly lib = Assembly.LoadFile(path);

        // Look up the class whose name was passed in as
        // an argument. If it is not a public class,
        // throw an exception.

        Type? t = lib.GetTypes().FirstOrDefault
            (t => t.FullName == className);
        if (t == null || !t.IsClass || !t.IsPublic)
            throw new ArgumentException
                ($"Type {className} not found or not a public class");

        // Make sure that the selected class implements the
        // specified public interface

        if (t.GetInterface(interfaceName) == null)
            throw new ArgumentException
                ($"Class {className} does not implement interface {interfaceName}");

        // We now create the instance of the concrete class
        // and return it as a reference to the desired interface.
        // May throw one of a number of exceptions, e.g. if the
        // concrete class has no default constructor.

        var instance = Activator.CreateInstance(t);
        if (instance is I interfaceInstance)
            return interfaceInstance;
        else
            throw new ArgumentException
                ($"Uanble to create instance implementing {interfaceName}");
    }
}
