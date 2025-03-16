namespace Kharazmi.AspNetCore.Core.IntegrationTests.Settings
{
    public enum ModelStateType
    {
        Success,
        Error,
        Info,
        Warning
    }

    public enum EditorType
    {
        DOCUPLOAD,
        NUMBERINPUT,
        DATEINPUT,
        BOOLINPUT,
        STRINGINPUT,
        KEYVALUE
    }

    public enum NumberType
    {
        None,
        INT,
        LONG,
        FLOAT,
        DOUBLE,
        DECIMAL 
    }
}