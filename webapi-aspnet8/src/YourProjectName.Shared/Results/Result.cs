﻿using System.Diagnostics.CodeAnalysis;
using FluentValidation.Results;

namespace YourProjectName.Shared.Results;

public class Result
{
    public Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None ||
            !isSuccess && error == Error.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error Error { get; }

    public static Result Ok() => new(true, Error.None);

    public static Result<TValue> Ok<TValue>(TValue value) =>
        new(value, true, Error.None);

    public static Result Fail(Error error) => new(false, error);

    public static Result<TValue> Fail<TValue>(Error error) =>
        new(default, false, error);
    public static Result<TValue> Fail<TValue>(IEnumerable<ValidationFailure> errors) =>
        new(default, false, new ValidationError(errors));
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    public Result(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    [NotNull]
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("The value of a failure result can't be accessed.");

    public static implicit operator Result<TValue>(TValue? value) =>
        value is not null ? Ok(value) : Fail<TValue>(Error.NullValue);

    public static Result<TValue> ValidationFailure(Error error) =>
        new(default, false, error);
}
