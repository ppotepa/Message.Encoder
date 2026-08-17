// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Assertion", "NUnit2005:Consider using Assert.That(actual, Is.EqualTo(expected)) instead of Assert.AreEqual(expected, actual)", Justification = "<Pending>", Scope = "member", Target = "~M:Message.Encoder.MessageBuilder.Tests.MessageBuilderTests.BinarySerializer_Works_Against_DefaultTextMessage")]
[assembly: SuppressMessage("Assertion", "NUnit2005:Consider using Assert.That(actual, Is.EqualTo(expected)) instead of Assert.AreEqual(expected, actual)", Justification = "<Pending>", Scope = "member", Target = "~M:Message.Encoder.MessageBuilder.Tests.MessageBuilderTests.Custom_Bit_Converter_Matches_Results_With_Built_In_One")]
[assembly: SuppressMessage("Assertion", "NUnit2005:Consider using Assert.That(actual, Is.EqualTo(expected)) instead of Assert.AreEqual(expected, actual)", Justification = "<Pending>", Scope = "member", Target = "~M:Message.Encoder.MessageBuilder.Tests.MessageBuilderTests.MessageBuilder_Serializes_Basic_Headers_Correctly")]
[assembly: SuppressMessage("Assertion", "NUnit2015:Consider using Assert.That(actual, Is.SameAs(expected)) instead of Assert.AreSame(expected, actual)", Justification = "<Pending>", Scope = "member", Target = "~M:Message.Encoder.MessageBuilder.Tests.MessageBuilderTests.UserIsNotAbleToOverrideHeaders")]
