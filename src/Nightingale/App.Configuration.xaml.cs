using CommunityToolkit.Diagnostics;
using CommunityToolkit.Extensions.DependencyInjection;
using JeniusApps.Common.Telemetry;
using JeniusApps.Common.Tools;
using JeniusApps.Common.Tools.Uwp;
using JeniusApps.Nightingale.Converters.Curl;
using JeniusApps.Nightingale.Converters.Insomnia;
using JeniusApps.Nightingale.Converters.OData;
using JeniusApps.Nightingale.Converters.Postman;
using JeniusApps.Nightingale.Converters.Swagger;
using Microsoft.Extensions.DependencyInjection;
using Nightingale.Auth;
using Nightingale.Core.Auth;
using Nightingale.Core.Certificates.Factories;
using Nightingale.Core.Client;
using Nightingale.Core.Client.Builders;
using Nightingale.Core.Client.Ssl;
using Nightingale.Core.CodeGenerators;
using Nightingale.Core.Cookies;
using Nightingale.Core.Dialogs;
using Nightingale.Core.Export;
using Nightingale.Core.Helpers;
using Nightingale.Core.Helpers.Interfaces;
using Nightingale.Core.Http;
using Nightingale.Core.Http.Interfaces;
using Nightingale.Core.ImportConverters.Nightingale;
using Nightingale.Core.Interfaces;
using Nightingale.Core.Mock.Services;
using Nightingale.Core.Services;
using Nightingale.Core.Settings;
using Nightingale.Core.Storage;
using Nightingale.Core.Storage.Interfaces;
using Nightingale.Core.ViewModels;
using Nightingale.Core.Workspaces.Factories;
using Nightingale.Core.Workspaces.Services;
using Nightingale.Dialogs;
using Nightingale.Handlers;
using Nightingale.History;
using Nightingale.HttpClients.RestSharpClient;
using Nightingale.Importers;
using Nightingale.Mock;
using Nightingale.Modifiers;
using Nightingale.Navigation;
using Nightingale.Tabs.Factories;
using Nightingale.Tabs.Services;
using Nightingale.Utilities;
using Nightingale.ViewModels;
using Nightingale.ViewModels.Factories;
using Nightingale.VisualState;
using System;

#nullable enable

namespace Nightingale;

partial class App
{
    private const string _rootParentId = "root";
    private IServiceProvider? _serviceProvider;

    public static IServiceProvider Services
    {
        get
        {
            IServiceProvider? serviceProvider = ((App)Current)._serviceProvider;

            if (serviceProvider is null)
            {
                ThrowHelper.ThrowInvalidOperationException("The service provider is not initialized");
            }

            return serviceProvider;
        }
    }

    private static IServiceProvider ConfigureServices(IStorage storage)
    {
        ServiceCollection collection = new();
        ConfigureServices(collection);
        collection.AddSingleton(storage);
        collection.AddSingleton<IRecentUrlCache, RecentUrlCache>(s => new RecentUrlCache(s.GetRequiredService<IRecentUrlStorageAccessor>(), _rootParentId));
        collection.AddSingleton<ITelemetry, SentryTelemetry>(s =>
        {
            string apiKey = s.GetRequiredService<IAppSettings>().TelemetryApiKey;
            bool isEnabled = s.GetRequiredService<IUserSettings>().Get<bool>(SettingsConstants.TelemetryEnabledKey);
            return new SentryTelemetry(apiKey, isEnabled);
        });

        IServiceProvider provider = collection.BuildServiceProvider();
        return provider;
    }

    /// <summary>
    /// Configures services used by the app.
    /// </summary>
    /// <param name="services">The target <see cref="IServiceCollection"/> instance to register services with.</param>
    [Singleton(typeof(ParameterStorageAccessor), typeof(IParameterStorageAccessor))]
    [Singleton(typeof(ApiTestStorageAccessor), typeof(IApiTestStorageAccessor))]
    [Singleton(typeof(AuthStorageAccessor), typeof(IAuthStorageAccessor))]
    [Singleton(typeof(RequestBodyStorageAccessor), typeof(IRequestBodyStorageAccessor))]
    [Singleton(typeof(RequestStorageAccessor), typeof(IRequestStorageAccessor))]
    [Singleton(typeof(EnvironmentStorageAccessor), typeof(IEnvironmentStorageAccessor))]
    [Singleton(typeof(CollectionStorageAccessor), typeof(ICollectionStorageAccessor))]
    [Singleton(typeof(WorkspaceItemStorageAccessor), typeof(IWorkspaceItemStorageAccessor))]
    [Singleton(typeof(WorkspaceStorageAccessor), typeof(IWorkspaceStorageAccessor))]
    [Singleton(typeof(RecentUrlStorageAccessor), typeof(IRecentUrlStorageAccessor))]
    [Singleton(typeof(EnvironmentContainer), typeof(IEnvironmentContainer))]
    [Singleton(typeof(CookieJar), typeof(ICookieJar))]
    [Singleton(typeof(FileWriter), typeof(IStorageFileWriter))]
    [Singleton(typeof(FileReader), typeof(IStorageFileReader))]
    [Singleton(typeof(UserSettings), typeof(IUserSettings))]
    [Singleton(typeof(AppSettings), typeof(IAppSettings))]
    [Singleton(typeof(LocalDeployService), typeof(IDeployService))]
    [Singleton(typeof(TabCollectionContainer), typeof(ITabCollectionContainer))]
    [Singleton(typeof(KbShortcutsHandler))]
    [Singleton(typeof(HttpClientFactory), typeof(IHttpClientFactory))]
    [Singleton(typeof(OAuth2ImplicitTokenRetriever), typeof(IImplicitTokenRetriver))]
    [Singleton(typeof(HistoryListModifier), typeof(IHistoryListModifier))]
    [Singleton(typeof(CurrentWorkspaceContainer), typeof(ICurrentWorkspaceContainer))]
    [Singleton(typeof(EnvFactory), typeof(IEnvFactory))]
    [Singleton(typeof(WorkspaceFactory), typeof(IWorkspaceFactory))]
    [Singleton(typeof(ItemFactory), typeof(IItemFactory))]
    [Singleton(typeof(WorkspaceTreeModifier), typeof(IWorkspaceTreeModifier))]
    [Singleton(typeof(DialogService), typeof(IDialogService))]
    [Singleton(typeof(MessageBus), typeof(IMessageBus))]
    [Singleton(typeof(EnvironmentListModifier), typeof(IEnvironmentListModifier))]
    [Singleton(typeof(WorkspaceListModifier), typeof(IWorkspaceListModifier))]
    [Singleton(typeof(VariableResolver), typeof(IVariableResolver))]
    [Singleton(typeof(WorkspaceNavigationService), typeof(IWorkspaceNavigationService))]
    [Singleton(typeof(WorkspaceItemNavigationService), typeof(IWorkspaceItemNavigationService))]
    [Singleton(typeof(RequestSender), typeof(IRequestSender))]
    [Singleton(typeof(NightingaleClient), typeof(INightingaleClient))]
    [Singleton(typeof(SslValidator), typeof(ISslValidator))]
    [Singleton(typeof(VisualStatePublisher), typeof(IVisualStatePublisher))]
    [Singleton(typeof(ResponseFileWriter), typeof(IResponseFileWriter))]
    [Singleton(typeof(ResponseValueExtractor), typeof(IResponseValueExtractor))]
    [Singleton(typeof(JsonValueExtractor), typeof(IJsonValueExtractor))]
    [Singleton(typeof(ExpressionTreeBuilder), typeof(IExpressionTreeBuilder))]
    [Singleton(typeof(RequestUrlBuilder), typeof(IRequestUrlBuilder))]
    [Singleton(typeof(StorageFilePicker), typeof(IFilePicker))]
    [Singleton(typeof(EnvironmentDialogService), typeof(IEnvironmentDialogService))]
    [Singleton(typeof(StorageExporter), typeof(IStorageExporter))]
    [Singleton(typeof(StorageImporter), typeof(IStorageImporter))]
    [Singleton(typeof(RootFrameNavigationService), typeof(IRootFrameNavigationService))]
    [Singleton(typeof(CurlGenerator), typeof(ICurlGenerator))]
    [Singleton(typeof(RestSharpCodeGenerator), typeof(IRestSharpCodeGenerator))]
    [Singleton(typeof(CodeGeneratorViewModelFactory), typeof(ICodeGeneratorViewModelFactory))]
    [Singleton(typeof(RestSharpMethodConverter), typeof(IHttpMethodConverter))]
    [Singleton(typeof(OneDriveSyncClient), typeof(ICloudBackupClient))]
    [Singleton(typeof(MethodsContainer), typeof(IMethodsContainer))]
    [Singleton(typeof(CookieDialogService), typeof(ICookieDialogService))]
    [Singleton(typeof(CertificateFactory), typeof(ICertificateFactory))]
    [Singleton(typeof(PostmanConverter), typeof(IPostmanConverter))]
    [Singleton(typeof(PostmanImporter), typeof(IPostmanImporter))]
    [Singleton(typeof(SwaggerConverter), typeof(ISwaggerConverter))]
    [Singleton(typeof(InsomniaConverterV4), typeof(IInsomniaConverter))]
    [Singleton(typeof(SwaggerImporter), typeof(ISwaggerImporter))]
    [Singleton(typeof(InsomniaImporter), typeof(IInsomniaImporter))]
    [Singleton(typeof(ODataConverter), typeof(IODataConverter))]
    [Singleton(typeof(ODataImporter), typeof(IODataImporter))]
    [Singleton(typeof(NcfImporter), typeof(INcfImporter))]
    [Singleton(typeof(WebAuthBroker), typeof(IWebAuthBroker))]
    [Singleton(typeof(OAuth1TokenRetriever), typeof(IOAuth1TokenRetriever))]
    [Singleton(typeof(BodyBuilder), typeof(IBodyBuilder))]
    [Singleton(typeof(HeaderBuilder), typeof(IHeaderBuilder))]
    [Singleton(typeof(FormDataBuilder), typeof(IFormDataBuilder))]
    [Singleton(typeof(OAuth2TokenRetriever), typeof(Core.Auth.IOAuth2TokenRetriever))]
    [Singleton(typeof(TabCollectionViewFactory), typeof(ITabCollectionViewFactory))]
    [Singleton(typeof(TabViewModelFactory), typeof(ITabViewModelFactory))]
    [Transient(typeof(BackgroundSettingsViewModel))]
    [Singleton(typeof(ExportService), typeof(IExportService))]
    [Singleton(typeof(CurlConverter), typeof(ICurlConverter))]
    [Singleton(typeof(MicrosoftStoreUpdater), typeof(IAppStoreUpdater))]
    [Transient(typeof(MainPageViewModel))]
    [Transient(typeof(ImportPostmanViewModel))]
    [Transient(typeof(SettingsViewModel))]
    [Transient(typeof(ExportControlViewModel))]
    [Transient(typeof(UrlBarViewModel))]
    [Transient(typeof(WorkspaceViewModel))]
    [Transient(typeof(RequestBodyViewModel))]
    [Transient(typeof(HistoryViewModel))]
    [Transient(typeof(AuthControlViewModel))]
    [Transient(typeof(BodyControlViewModel))]
    [Transient(typeof(StatusBarViewModel))]
    [Transient(typeof(CodeGenPageViewModel))]
    [Transient(typeof(CollectionViewModel))]
    [Transient(typeof(RequestPageViewModel))]
    [Transient(typeof(EnvironmentsViewModel))]
    [Transient(typeof(RequestControlViewModel))]
    [Transient(typeof(RestSharpAuthenticator), typeof(Core.Interfaces.IOAuth2TokenRetriever))]
    [Transient(typeof(WorkspaceResponseLogger), typeof(ILogger))]
    [Singleton(typeof(StoreHandler), typeof(IStoreHandler))]
    private static partial void ConfigureServices(IServiceCollection services);
}
