using System;

// 実行方法
// (Add-Type -Path "SwingConverter.cs" -PassThru)::Main()

public class SwingConverter
{
    public static void Main()
    {
        // Console.WriteLine("Hello World");

        FileUtils.ReadFile(handler => {
            Console.WriteLine(handler.ReadLine());
        },
        "SampleSwing.anim",
        "../Resources/Animations"
        );
    }
}