using System.Text.Json.Serialization;

public class DataDesensitiveConverter : JsonConverter<string>
{
    private readonly char PaddingChar;
    private readonly int PreHold, EndHold;

    /// <summary>
    /// 数据脱敏
    /// </summary>
    /// <param name="paddingChar">占位符</param>
    /// <param name="preHold">头部字符保留长度</param>
    /// <param name="endHold">尾部字符保留长度</param>
    public DataDesensitiveConverter(char paddingChar,int preHold, int endHold)
    {
        PaddingChar = paddingChar;
        PreHold= preHold;
        EndHold = endHold;
    }

    /// <summary>
    /// 数据脱敏,默认占位符  *， 首尾各留4字符，中间字符加密
    /// </summary>
    public DataDesensitiveConverter()
    {
        PaddingChar = '*';
        PreHold = 4;
        EndHold = 4;
    }

    public override void Write(Utf8JsonWriter writer, string data, JsonSerializerOptions options)
    {
        if(string.IsNullOrEmpty(data))
        {
            writer.WriteStringValue(data);
            return;
        }
        if (PreHold + EndHold >= data.Length)
        {
            if (PreHold >= data.Length)
            {
                data = "".PadRight(data.Length, PaddingChar);
            }
            else
            {
                data = data.Substring(0, PreHold).PadRight(data.Length - PreHold);
            }
        }
        else
        {
            string start = data.Substring(0, PreHold);
            string end = data.Substring(data.Length - EndHold);
            string center = "".PadRight(data.Length - (PreHold + EndHold), PaddingChar);
            data = $"{start}{center}{end}";
        }
        writer.WriteStringValue(data);
    }
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString();
    }

}