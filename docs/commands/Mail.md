---
layout: default
title: Mail
parent: Commands
---

<details open markdown="block">
  <summary>
    Table of contents
  </summary>
  {: .text-delta }
- TOC
{:toc}
</details>

# Mail

CatCore's mail system is designed to distrubute important messages (like new updates or downtime) to it's users. The bot will automatically notify you when you have unread mail.

![unread message notification](images/mail_unread.png)

Mail groups are used to determines what notifications you will get (for example if you want to know about every new feature the moment it drops you could subscribe to the `patchnotes` feed).

## Mail Inbox
{: .d-inline-block }

Slash Command
{: .label .label-green }

```xml
/mail inbox <filter> <message> 
```

| arg     | type                                               | use                                     |
|---------|----------------------------------------------------|-----------------------------------------|
| filter  | Mail Search Filter                                 | Chooses the message to show             |
| message | [Mail Message](index.md#autocomplete-option-types) | Determines what message you want to see |

This command lets you search the contents of your inbox and select a specific message to view. Viewing a message for the first time will mark it as read so it's easier to find mesages that need your attention. 

## Mail Join
{: .d-inline-block }

Slash Command
{: .label .label-green }

```xml
/mail join <group>
```

| arg   | type                                                | use               |
|-------|-----------------------------------------------------|-------------------|
| group | [Message Group](index.md#autocomplete-option-types) | The group to join |

This command will subscribe you to a message group, delivering all of it's new messages to your inbox. You will not older messages from the group.

## Mail Leave
{: .d-inline-block }

Slash Command
{: .label .label-green }

```xml
/mail leave <group>
```

| arg   | type                                                  | use                |
|-------|-------------------------------------------------------|--------------------|
| group | [Message Group](index.md#autocomplete-option-types) | The group to leave |

This command will unsubscribe you to a message group, dissabling delivery of it's messages. Messages you have received from the group will not be deleted.
