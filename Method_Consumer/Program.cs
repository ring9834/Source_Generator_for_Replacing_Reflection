
using Method_Consumer;

class Program
{
    static void Main()
    {
        var p = new Person();
        Console.WriteLine(p.HiFromGeneratedCode());
        // Output: Hi from Source Generator! (generated for Person at compile-time)
    }
}