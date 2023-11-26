namespace FsmWorkFlowUI.Data;

public class ExampleModel
{
    public static readonly string[] Analytes =
    {
        "Glucose",
        "Analyte2"
    };

    private string? analyte;
    public string? Analyte
    {
        get => analyte;
        set
        {
            analyte = value;
            InvalidateKeys();
        }
    }

    private int? batchNumber;
    public int? BatchNumber 
    {  
        get => batchNumber; 
        set
        {
            batchNumber = value;
            InvalidateKeys();
        }
    }   

    public string? FirstSignature { get; set; }

    public long FirstUserKey { get; set; } = 0;

    public string? SecondSignature { get; set; }

    public long SecondUserKey { get; set; } = 0;

    public int? ResultId { get; set; }

    // Validation methods

    public bool ValidBatch 
        => BatchNumber != null && BatchNumber > 0;

    public bool ValidAnalyte
        => Analyte != null && Analytes.Contains(Analyte);

    private void InvalidateKeys()
    {
        FirstUserKey = 0;
        SecondUserKey = 0;
    }
}
