using VueAppAdmin.Server.Features.FeatureList.Responses;

namespace VueAppAdmin.Server.Features.FeatureList;

public interface IFeaturesService
{
    IEnumerable<FeatureResponse> GetAll();
}
