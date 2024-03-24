using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;

namespace Nightingale
{
    public static class Program
    {
        // This example code shows how you could implement the required Main method to
        // support multi-instance redirection. The minimum requirement is to call
        // Application.Start with a new App object. Beyond that, you may delete the
        // rest of the example code and replace it with your custom code if you wish.

        static void Main(string[] args)
        {
            // First, we'll get our activation event args, which are typically richer
            // than the incoming command-line args. We can use these in our app-defined
            // logic for generating the key for this instance.
            IActivatedEventArgs activatedArgs = AppInstance.GetActivatedEventArgs();

            if (activatedArgs is FileActivatedEventArgs fileArgs)
            {
                IStorageItem file = fileArgs.Files.FirstOrDefault();
                if (file == null)
                {
                    return;
                }

                var instance = AppInstance.FindOrRegisterInstanceForKey(file.Path);

                if (instance.IsCurrentInstance)
                {
                    global::Windows.UI.Xaml.Application.Start((p) => new App());
                }
                else
                {
                    instance.RedirectActivationTo();
                }
            }
            else
            {
                // If the Windows shell indicates a recommended instance, then
                // the app can choose to redirect this activation to that instance instead.
                if (AppInstance.RecommendedInstance != null)
                {
                    AppInstance.RecommendedInstance.RedirectActivationTo();
                }
                else
                {
                    // Define a key for this instance, based on some app-specific logic.
                    // If the key is always unique, then the app will never redirect.
                    // If the key is always non-unique, then the app will always redirect
                    // to the first instance. In practice, the app should produce a key
                    // that is sometimes unique and sometimes not, depending on its own needs.
                    string key = "MAIN"; 
                    var instance = AppInstance.FindOrRegisterInstanceForKey(key);
                    if (instance.IsCurrentInstance)
                    {
                        // If we successfully registered this instance, we can now just
                        // go ahead and do normal XAML initialization.
                        global::Windows.UI.Xaml.Application.Start((p) => new App());
                    }
                    else
                    {
                        // Some other instance has registered for this key, so we'll 
                        // redirect this activation to that instance instead.
                        instance.RedirectActivationTo();
                    }
                }
            }
        }
    }
}
