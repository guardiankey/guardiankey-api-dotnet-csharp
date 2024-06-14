# Introduction

This is a GuardianKey API implementation in C#.

# Usage

- GuardianKey.cs -- library
- Program.cs -- usage example

You must set the deploy data (variables below in the file `Program.cs`). This information can be found in the GuardianKey's administration panel, authgroup settings.

```
        gkConf.Add("organization_id", "");
        gkConf.Add("authgroup_id", "");
        gkConf.Add("key", "");
        gkConf.Add("iv", "");

