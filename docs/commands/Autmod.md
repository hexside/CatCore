---
layout: default
title: Automod
parent: Commands
---

# Automod
Automoderation commands provide a framework for blocking certian types of unwanted content in your Discord community, with build in Regex matching and a versatile permission system (comming soon™️) these utilities will keep your community safe from even the most presistent chronicly online 20 year olds reminding you of their favorite Linux flavor (It's Arch btw).

## Automod Notifications 
{: .d-inline-block }
Slash Command
{: .label .label-green }

```xml
/automod notifications <chanmel> 
```

| arg    | type         | use                                              |
|--------|--------------|--------------------------------------------------|
| chanel | Text Channel | The channel notifications should be delivered to |

This command specifies what channels notifications about automod actions will be sent to. These notifications include flagged messages and delted scam links. If the channel is deleted some moderation functions may fail and message flagging will no longer function.
