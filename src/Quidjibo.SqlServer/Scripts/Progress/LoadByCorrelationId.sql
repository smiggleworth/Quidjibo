SELECT [Id], [WorkId], [CorrelationId], [Name], [Queue], [Note], [Value], [RecordedOn]
FROM [Quidjibo].[Progress] 
WHERE [CorrelationId] = @CorrelationId