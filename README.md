Portkit
===

[![Build Status](https://travis-ci.org/bagabont/portkit.svg?branch=master)](https://travis-ci.org/bagabont/portkit)
___


The package contains base components that are capable of providing multi-platform caching, simple cryptography, collection and routines extensions, dispatching asynchronously messages to a synchronization context. It also includes implementation layer for working with Model-View-ViewModel(MVVM) based infrastructures. Main interfaces for the most commonly used services are also included in the library. Inside you will find an implementation of the ICommand with async actions supported, light abstract implementation of the INotifyPropertyChanged and property validation. There is also a basic implementation of a message bus service for decoupled messaging between presenter models.

To install Portkit packages, run the each of the following commands in the `Package Manager Console`

```powershell
PM> Install-Package Portkit.Core
PM> Install-Package Portkit.Component
PM> Install-Package Portkit.Logging
```
