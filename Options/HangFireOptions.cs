using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace Authorization.Options;

public class HangFireOptions
{
    [Required] public string JobStorage { get; set; } = default!;
    [Required] public string ServerUrl { get; set; }  = default!;
    public string QueryString { get; set; } = default!;
}
public class HangFireOptionsConfiguration:IConfigureOptions<HangFireOptions>
{
    public void Configure(HangFireOptions options)
    {
        options.QueryString = "TestQueue";
    }
    
}