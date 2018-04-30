using Raven.Client.Documents.Indexes;
using Raven.Client.ServerWide.Operations.Certificates;

namespace Raven.Client.Documents.Operations
{
    public abstract class ResultsResponse<T>
    {
        public T[] Results { get; set; }
    }

    public class GetIndexNamesResponse : ResultsResponse<string>
    {
    }

    public class PutIndexesResponse : ResultsResponse<PutIndexResult>
    {
    }

    public class GetIndexesResponse : ResultsResponse<IndexDefinition>
    {
    }

    public class GetIndexStatisticsResponse : ResultsResponse<IndexStats>
    {
    }

    public class GetCertificatesResponse : ResultsResponse<CertificateDefinition>
    {
    }

    public class GetClientCertificatesResponse : ResultsResponse<CertificateRawData>
    {
    }
}
