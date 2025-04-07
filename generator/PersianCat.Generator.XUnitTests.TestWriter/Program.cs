
using PersianCat.Generator.XUnitTest.Common;

Console.WriteLine("Writing XUnite tests to files is begging!");

GeneratedXUnitTestHelper.SaveGeneratedUnitTests(searchPattern: "Kharazmi.*.dll",
    changeBasePath: static (x) => System.IO.Path.Combine(x.BasePath.Replace("src", "", StringComparison.OrdinalIgnoreCase), "test"),
    onSave: Console.WriteLine,
    onError: (m, e) => Console.WriteLine($"{m}\n{e.Message}"));

