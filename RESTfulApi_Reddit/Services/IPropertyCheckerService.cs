namespace RESTfulApi_Reddit.Services {
    public interface IPropertyCheckerService {
        bool TypeHasProperties<T>(string fields);
    }
}