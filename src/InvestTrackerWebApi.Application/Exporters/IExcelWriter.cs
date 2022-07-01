namespace InvestTrackerWebApi.Application.Exporters;
using System.Collections.Generic;

public interface IExcelWriter
{
    Stream WriteToStream<T>(IList<T> data);
}
