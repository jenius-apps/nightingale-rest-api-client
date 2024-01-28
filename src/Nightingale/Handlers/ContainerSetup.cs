using JeniusApps.Nightingale.Converters.Curl;
using JeniusApps.Nightingale.Converters.Insomnia;
using JeniusApps.Nightingale.Converters.OData;
using JeniusApps.Nightingale.Converters.Postman;
using JeniusApps.Nightingale.Converters.Swagger;
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

namespace Nightingale.Handlers
{
    //public class ContainerSetup
    //{
    //    private const string _rootParentId = "root";

    //    public static IContainer Create(IStorage storage)
    //    {
    //        var builder = new ContainerBuilder();

    //        builder.RegisterInstance(storage).As<IStorage>().SingleInstance();

    //        builder.RegisterType<ParameterStorageAccessor>().As<IParameterStorageAccessor>().SingleInstance();
    //        builder.RegisterType<ApiTestStorageAccessor>().As<IApiTestStorageAccessor>().SingleInstance();
    //        builder.RegisterType<AuthStorageAccessor>().As<IAuthStorageAccessor>().SingleInstance();
    //        builder.RegisterType<RequestBodyStorageAccessor>().As<IRequestBodyStorageAccessor>().SingleInstance();
    //        builder.RegisterType<RequestStorageAccessor>().As<IRequestStorageAccessor>().SingleInstance();
    //        builder.RegisterType<EnvironmentStorageAccessor>().As<IEnvironmentStorageAccessor>().SingleInstance();
    //        builder.RegisterType<CollectionStorageAccessor>().As<ICollectionStorageAccessor>().SingleInstance();
    //        builder.RegisterType<WorkspaceItemStorageAccessor>().As<IWorkspaceItemStorageAccessor>().SingleInstance();
    //        builder.RegisterType<WorkspaceStorageAccessor>().As<IWorkspaceStorageAccessor>().SingleInstance();
    //        builder.RegisterType<RecentUrlStorageAccessor>().As<IRecentUrlStorageAccessor>().SingleInstance();
    //        builder.RegisterType<RecentUrlCache>().As<IRecentUrlCache>().SingleInstance()
    //            .WithParameter("rootParentId", _rootParentId);
    //        builder.RegisterType<EnvironmentContainer>().As<IEnvironmentContainer>().SingleInstance();
    //        builder.RegisterType<CookieJar>().As<ICookieJar>().SingleInstance();
    //        builder.RegisterType<FileWriter>().As<IStorageFileWriter>().SingleInstance();
    //        builder.RegisterType<FileReader>().As<IStorageFileReader>().SingleInstance();
    //        builder.RegisterType<UserSettings>().As<IUserSettings>().SingleInstance();
    //        builder.RegisterType<AppSettings>().As<IAppSettings>().SingleInstance();

    //        builder.RegisterType<LocalDeployService>().As<IDeployService>().InstancePerLifetimeScope();
    //        builder.RegisterType<TabCollectionContainer>().As<ITabCollectionContainer>().InstancePerLifetimeScope();
    //        builder.RegisterType<KbShortcutsHandler>().InstancePerLifetimeScope();
    //        builder.RegisterType<HttpClientFactory>().As<IHttpClientFactory>().InstancePerLifetimeScope();
    //        builder.RegisterType<OAuth2ImplicitTokenRetriever>().As<IImplicitTokenRetriver>().InstancePerLifetimeScope();
    //        builder.RegisterType<HistoryListModifier>().As<IHistoryListModifier>().InstancePerLifetimeScope();
    //        builder.RegisterType<CurrentWorkspaceContainer>().As<ICurrentWorkspaceContainer>().InstancePerLifetimeScope();
    //        builder.RegisterType<EnvFactory>().As<IEnvFactory>().InstancePerLifetimeScope();
    //        builder.RegisterType<WorkspaceFactory>().As<IWorkspaceFactory>().InstancePerLifetimeScope();
    //        builder.RegisterType<ItemFactory>().As<IItemFactory>().InstancePerLifetimeScope();
    //        builder.RegisterType<WorkspaceTreeModifier>().As<IWorkspaceTreeModifier>().InstancePerLifetimeScope();
    //        builder.RegisterType<DialogService>().As<IDialogService>().InstancePerLifetimeScope();
    //        builder.RegisterType<MessageBus>().As<IMessageBus>().InstancePerLifetimeScope();
    //        builder.RegisterType<EnvironmentListModifier>().As<IEnvironmentListModifier>().InstancePerLifetimeScope();
    //        builder.RegisterType<WorkspaceListModifier>().As<IWorkspaceListModifier>().InstancePerLifetimeScope();
    //        builder.RegisterType<VariableResolver>().As<IVariableResolver>().InstancePerLifetimeScope();
    //        builder.RegisterType<WorkspaceNavigationService>().As<IWorkspaceNavigationService>().InstancePerLifetimeScope();
    //        builder.RegisterType<WorkspaceItemNavigationService>().As<IWorkspaceItemNavigationService>().InstancePerLifetimeScope();
    //        builder.RegisterType<RequestSender>().As<IRequestSender>().InstancePerLifetimeScope();
    //        builder.RegisterType<NightingaleClient>().As<INightingaleClient>().InstancePerLifetimeScope();
    //        builder.RegisterType<SslValidator>().As<ISslValidator>().InstancePerLifetimeScope();
    //        builder.RegisterType<VisualStatePublisher>().As<IVisualStatePublisher>().InstancePerLifetimeScope();
    //        builder.RegisterType<ResponseFileWriter>().As<IResponseFileWriter>().InstancePerLifetimeScope();
    //        builder.RegisterType<ResponseValueExtractor>().As<IResponseValueExtractor>().InstancePerLifetimeScope();
    //        builder.RegisterType<JsonValueExtractor>().As<IJsonValueExtractor>().InstancePerLifetimeScope();
    //        builder.RegisterType<ExpressionTreeBuilder>().As<IExpressionTreeBuilder>().InstancePerLifetimeScope();
    //        builder.RegisterType<RequestUrlBuilder>().As<IRequestUrlBuilder>().InstancePerLifetimeScope();
    //        builder.RegisterType<StorageFilePicker>().As<IFilePicker>().InstancePerLifetimeScope();
    //        builder.RegisterType<EnvironmentDialogService>().As<IEnvironmentDialogService>().InstancePerLifetimeScope();
    //        builder.RegisterType<StorageExporter>().As<IStorageExporter>().InstancePerLifetimeScope();
    //        builder.RegisterType<StorageImporter>().As<IStorageImporter>().InstancePerLifetimeScope();
    //        builder.RegisterType<RootFrameNavigationService>().As<IRootFrameNavigationService>().InstancePerLifetimeScope();
    //        builder.RegisterType<CurlGenerator>().As<ICurlGenerator>().InstancePerLifetimeScope();
    //        builder.RegisterType<RestSharpCodeGenerator>().As<IRestSharpCodeGenerator>().InstancePerLifetimeScope();
    //        builder.RegisterType<CodeGeneratorViewModelFactory>().As<ICodeGeneratorViewModelFactory>().InstancePerLifetimeScope();
    //        builder.RegisterType<RestSharpMethodConverter>().As<IHttpMethodConverter>().InstancePerLifetimeScope();
    //        builder.RegisterType<OneDriveSyncClient>().As<ICloudBackupClient>().InstancePerLifetimeScope();
    //        builder.RegisterType<MethodsContainer>().As<IMethodsContainer>().InstancePerLifetimeScope();
    //        builder.RegisterType<MvpViewModel>().InstancePerLifetimeScope();
    //        builder.RegisterType<CookieDialogService>().As<ICookieDialogService>();
    //        builder.RegisterType<CertificateFactory>().As<ICertificateFactory>();
    //        builder.RegisterType<PostmanConverter>().As<IPostmanConverter>();
    //        builder.RegisterType<PostmanImporter>().As<IPostmanImporter>();
    //        builder.RegisterType<SwaggerConverter>().As<ISwaggerConverter>();
    //        builder.RegisterType<InsomniaConverterV4>().As<IInsomniaConverter>();
    //        builder.RegisterType<SwaggerImporter>().As<ISwaggerImporter>();
    //        builder.RegisterType<InsomniaImporter>().As<IInsomniaImporter>();
    //        builder.RegisterType<ODataConverter>().As<IODataConverter>();
    //        builder.RegisterType<ODataImporter>().As<IODataImporter>();
    //        builder.RegisterType<NcfImporter>().As<INcfImporter>();
    //        builder.RegisterType<WebAuthBroker>().As<IWebAuthBroker>();
    //        builder.RegisterType<OAuth1TokenRetriever>().As<IOAuth1TokenRetriever>();
    //        builder.RegisterType<BodyBuilder>().As<IBodyBuilder>();
    //        builder.RegisterType<HeaderBuilder>().As<IHeaderBuilder>();
    //        builder.RegisterType<FormDataBuilder>().As<IFormDataBuilder>();
    //        builder.RegisterType<OAuth2TokenRetriever>().As<Core.Auth.IOAuth2TokenRetriever>();
    //        builder.RegisterType<TabCollectionViewFactory>().As<ITabCollectionViewFactory>();
    //        builder.RegisterType<TabViewModelFactory>().As<ITabViewModelFactory>();
    //        builder.RegisterType<BackgroundSettingsViewModel>().InstancePerLifetimeScope();
    //        builder.RegisterType<ExportService>().As<IExportService>();

    //        builder.RegisterType<CurlConverter>().As<ICurlConverter>();
    //        builder.RegisterType<MainPageViewModel>();
    //        builder.RegisterType<ImportPostmanViewModel>();
    //        builder.RegisterType<SettingsViewModel>();
    //        builder.RegisterType<ExportControlViewModel>().InstancePerLifetimeScope();
    //        builder.RegisterType<UrlBarViewModel>();
    //        builder.RegisterType<WorkspaceViewModel>();
    //        builder.RegisterType<RequestBodyViewModel>();
    //        builder.RegisterType<HistoryViewModel>();
    //        builder.RegisterType<AuthControlViewModel>();
    //        builder.RegisterType<BodyControlViewModel>();
    //        builder.RegisterType<StatusBarViewModel>();
    //        builder.RegisterType<CodeGenPageViewModel>();
    //        builder.RegisterType<CollectionViewModel>();
    //        builder.RegisterType<RequestPageViewModel>();
    //        builder.RegisterType<EnvironmentsViewModel>();
    //        builder.RegisterType<RequestControlViewModel>();
    //        builder.RegisterType<RestSharpAuthenticator>().As<Core.Interfaces.IOAuth2TokenRetriever>().AsSelf();
    //        builder.RegisterType<WorkspaceResponseLogger>().As<ILogger>();

    //        return builder.Build();
    //    }
    //}
}
