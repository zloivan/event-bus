# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2024-07-08
### Added
- Initial release of the Event Bus System for Unity.
- Added `IEvent` interface for event definition.
- Added `IEventBinding<T>` interface for event binding.
- Implemented `EventBus<T>` class for managing event registration, deregistration, and invocation.
- Implemented `EventBinding<T>` class for event handling.
- Added `EventBusLogger` class for logging with support for different log levels.
- Added `EventBusUtilities` class for automatic event bus initialization and cleanup.
- Packaged optional sample scripts in the project.