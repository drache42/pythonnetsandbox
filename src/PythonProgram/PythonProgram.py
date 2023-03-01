
# Load python net and the clr
from ast import Try
from pythonnet import load
# load must be called before import clr
load("coreclr")
import clr

import sys
import pathlib
from os.path import abspath

# Get the path the dist directory so we can load the dotnet dlls
# There might be a better way to do this, needs more research
distPath = abspath(pathlib.Path(__file__).parent.resolve().__str__() + "../../../dist")
sys.path.append(distPath)

# Load that dlls that export the objects we need. This is equivilant to "Add Reference" in Visual Studio C# solutions
#
# Add StringPrinterSDK - the project working dir the dist folder, so `dotnet publish` needs to be run on the dotnet projects
# before this will work. That step is not yet automated and must be run if the sdk changes
clr.AddReference("StringPrinterSDK")

# Load references to all the microsoft projects we need
clr.AddReference("Microsoft.Extensions.DependencyInjection")
clr.AddReference("Microsoft.Extensions.Hosting")
clr.AddReference("System.Runtime")
clr.AddReference("System")
clr.AddReference("Microsoft.Extensions.Logging")
clr.AddReference("Microsoft.Extensions.Logging.Configuration")

# This block tells python which objects to load from the references above. 
# This is similar to "using namespace" in C#, except that it also loads the objects after the `import` statement
#from StringPrinterSDK import IStringPrinter, StringPrinter, PrePrintEventArgs, PostPrintEventArgs, IPrintObject, PythonLogger, PythonLoggerExtensions, PythonLoggerProvider, IServiceProviderExtensions
#from Microsoft.Extensions.Hosting import *
#from System import Exception, Action, Func, IServiceProvider, String, ApplicationException
#from Microsoft.Extensions.DependencyInjection import IServiceCollection, LoggingServiceCollectionExtensions, ServiceProviderServiceExtensions
#from Microsoft.Extensions.Logging import ILoggingBuilder, LoggingBuilderExtensions

import StringPrinterSDK
import System
import Microsoft.Extensions.Hosting
import Microsoft.Extensions.DependencyInjection
import Microsoft.Extensions.Logging
import System.Threading.Tasks

#from StringPrinterSDK import IStringPrinter, StringPrinter, PrePrintEventArgs, PostPrintEventArgs, IPrintObject, PythonLogger, PythonLoggerExtensions, PythonLoggerProvider, IServiceProviderExtensions
#from Microsoft.Extensions.Hosting import *
#from System import Exception, Action, Func, IServiceProvider, String, ApplicationException
#from Microsoft.Extensions.DependencyInjection import IServiceCollection, LoggingServiceCollectionExtensions, ServiceProviderServiceExtensions
#from Microsoft.Extensions.Logging import ILoggingBuilder, LoggingBuilderExtensions

import logging

# Python and python.net has some quality of live drawbacks compared to C#
# It can't call extension method directly on the parent object
# and it can't really do inline lambda 
# Because of this, we need to first create a function that serves in place of a lambda
# and when we want to call an extension method, we need to call the static extension method through the extension class (like any other static class method)

def configureServicesDelegate(services: Microsoft.Extensions.DependencyInjection.IServiceCollection):
    '''
    Configure the services for the IHostBuilder
    '''
    # Remove logging because it won't work here
    Microsoft.Extensions.DependencyInjection.LoggingServiceCollectionExtensions.AddLogging(services, System.Action[Microsoft.Extensions.Logging.ILoggingBuilder](AddLoggingDelegate))
    
    # Add the StringPrinterSDK
    StringPrinterSDK.IServiceProviderExtensions.AddStringPrinterSDK(services)


def AddLoggingDelegate(builder: Microsoft.Extensions.Logging.ILoggingBuilder):
    '''
    Remove the default logging providers. 
    Other the following exception is thrown
    "System.Diagnostics.EventLog access is not supported on this platform."

    EventLog is added by default on Windows only (https://learn.microsoft.com/en-us/dotnet/core/extensions/logging-providers)
    I assume what's happening is that it's checks if it's running on windows, if so it adds EventLog, but python being cross-platform
    doesn't like that, so it throws. More research is required to confirm that hypothesis. Either way, this needs to be done.
    '''
    Microsoft.Extensions.Logging.LoggingBuilderExtensions.ClearProviders(builder)
    StringPrinterSDK.PythonLoggerExtensions.AddPythonLogger(builder, System.Func[System.IServiceProvider, StringPrinterSDK.PythonLoggerProvider](GetNewPythonLoggingProviderDelegate))


def GetNewPythonLoggerDelegate(name):
    '''
    Simply returns the python implemented PyLogger
    '''
    return PyLogger(name)


def GetNewPythonLoggingProviderDelegate(services: Microsoft.Extensions.DependencyInjection.IServiceCollection):
    '''
    Simply returns a new PythonLoggerProvider
    '''
    return StringPrinterSDK.PythonLoggerProvider(System.Func[System.String, StringPrinterSDK.PythonLogger](GetNewPythonLoggerDelegate))
    

def StringBuilder_PrePrint(_, e: StringPrinterSDK.PrePrintEventArgs):
    '''
    Pre print event handler
    '''
    print("My pre print event handler: " + e.PrePrintString)


def StringBuilder_PostPrint(_, e: StringPrinterSDK.PostPrintEventArgs):
    '''
    Post print event handler
    '''
    print("My post print event handler: " + e.PostPrintString)


class PrintObject(StringPrinterSDK.IPrintObject):
    __namespace__ = "StringPrinterSDK" # Must match the namespace of the interface

    # Define the getter for "Data"
    # This can be found by running `dir(<interface>)` to see what methods are exposes that need to be implemented
    def get_Data(self):
        return "Python Print Object Data"


class PyLogger(StringPrinterSDK.PythonLogger):
    __namespace__ = "StringPrinterSDK"

    def __init__(self, name):
        self._logger = logging.getLogger(name.upper())
        handler = logging.StreamHandler()
        formatter = logging.Formatter('%(asctime)s - %(name)s - %(levelname)s - %(message)s')
        handler.setFormatter(formatter)
        self._logger.addHandler(handler)
        self._logger.setLevel(logging.INFO)


    def Log(self, logLevel, eventId, state, exception, message):
        if(self.IsEnabled(logLevel) is False):
            return # shortcut

        # Python log level are 10x the C# level, multiplying here and passing along the message
        # Python: https://docs.python.org/3/howto/logging.html#logging-levels
        # C#: https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loglevel?view=dotnet-plat-ext-6.0
        self._logger.log(int(logLevel) * 10, message);


    def IsEnabled(self, logLevel):
        # Verify that message log level is same or above the logger, else short circuit
        return (int(logLevel) * 10 >= self._logger.level)


# Playing around with wrapping async function in python sync functions
class PyStringPrinter(StringPrinterSDK.StringPrinter):

    def __init__(self, StringPrinter):
        None

    def PrintSync(self):
        try:
            t = StringPrinterSDK.StringPrinter.AsyncPrintWillThrow()
            a = t.GetAwaiter()
            r = a.GetResult()

            return r
        except System.Exception as e:
            print(f"caught the exception we expected. {e.Message}")
        except TypeError as e:
            print(f"caught the exception we did not expected. {e}")


# Build the DI container. Because extension method can't be used directly, it's not as pretty as in C#
hostbuilder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
Microsoft.Extensions.Hosting.HostingHostBuilderExtensions.ConfigureServices(hostbuilder, System.Action[Microsoft.Extensions.DependencyInjection.IServiceCollection](configureServicesDelegate))
host = hostbuilder.Build()

# Get the IStringPrinter from the DI host
stringPrinter = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService[StringPrinterSDK.IStringPrinter](host.Services)

# Hook up pre and post print events
stringPrinter.PrePrint += StringBuilder_PrePrint
stringPrinter.PostPrint += StringBuilder_PostPrint

# Normal Print
try:
    result = stringPrinter.Print()
    print(f"Succeeded: {result.Succeeded}")

    # Object Print
    printObject = PrintObject()
    result = stringPrinter.Print(printObject)
    print(f"Succeeded: {result.Succeeded}")
except System.Exception as e:
    print(f"{e.Message}")


# Async Print
result = stringPrinter.AsyncPrint().GetAwaiter().GetResult()
print(f"Succeeded: {result.Succeeded}")

# Throw an exception
try:
    result = stringPrinter.Print(None)
    print(f"Succeeded: {result.Succeeded}")
except:
    print(f"Yay an exception!")

# Python sync wrapper stuff
pyStringPrinter = PyStringPrinter(stringPrinter)
pyStringPrinter.Print()         # calls default print function
pyStringPrinter.PrintSync()     # calls async wrapped function with try/except