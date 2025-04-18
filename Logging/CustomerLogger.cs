namespace APICatalogo.Logging;

public class CustomerLogger : ILogger
{
    readonly string loggerName;

    readonly CustomLoggerProviderConfiguration loggerConfig;

    public CustomerLogger(string name, CustomLoggerProviderConfiguration config)
    {
        loggerName = name;
        loggerConfig = config;
    }   

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel == loggerConfig.LogLevel;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return null;
    }


    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
        Exception exception, Func<TState, Exception, string> formatter)
    {
        string mensagem = ($"Log: {logLevel} - {eventId} - {state} - {exception?.Message} - {formatter(state, exception)}");

        //EscreverTextoNoArquivo(mensagem);
    }

    private void EscreverTextoNoArquivo(string mensagem)
    {
        //string caminho = Path.Combine(Directory.GetCurrentDirectory(), "Logs", "log.txt");
        //if (!File.Exists(caminho))
        //{
        //    File.Create(caminho).Close();
        //}
        //using (StreamWriter sw = File.AppendText(caminho))
        //{
        //    sw.WriteLine(mensagem);
        //    sw.Flush();
        //    sw.Close();
        //}

        // Representação de mensagem sendo escrita
    }
}
