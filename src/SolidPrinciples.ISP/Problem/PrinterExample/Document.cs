namespace SolidPrinciples.ISP.Problem.PrinterExample;

/// <summary>
/// Clase simple que representa un documento.
/// </summary>
public sealed class Document
{
    public string Name { get; }
    public string Content { get; }

    public Document(string name, string content)
    {
        Name = name;
        Content = content;
    }
}
