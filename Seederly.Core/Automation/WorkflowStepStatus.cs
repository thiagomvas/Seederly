namespace Seederly.Core.Automation;

public enum WorkflowStepStatus
{
    Success,
    ExtractionFailure,
    InjectionFailure,
    ApiFailure,
    WorkflowFailure,
    VariableNotFound,
    EndpointNotFound,
    InternalError,
}