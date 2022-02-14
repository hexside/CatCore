---
layout: default
title: Poll
parent: Commands
has_children: true
---

<details open markdown="block">
  <summary>
    Table of contents
  </summary>
  {: .text-delta }
- TOC
{:toc}
</details>

# Poll
A poll is a menu created by CatCore that lets your members assign roles to themselves. Members must have the ``Manage Roles`` permission to create and modify polls.

## Poll New 
{: .d-inline-block }
Slash Command
{: .label .label-green }
Modal
{: .label .label-pink }

```xml
/poll new <min>? <max>?
```

| arg | type | use                                                                                                      |
|-----|------|----------------------------------------------------------------------------------------------------------|
| min | Int? | The smallest number of roles a user can have from the poll, defaults to 1 if the poll has too few roles. |
| max | Int? | The largest number of roles a user can have from the poll, defaults to the total number of roles.        |

This command creates a new poll with the specified attributes, by default polls have no roles, in order to add them run [``/poll add-role``](#poll-add-role). To test your poll run [``/poll send``](#poll-send) in a private channel. To modify a poll after creating it run [``/poll update``](#poll-update). This command will respond with a [poll creation modal](modals.md#creating-and-updating-polls) which you must fill out.

## Poll Delete
{: .d-inline-block }
Slash Command
{: .label .label-green }

```xml
/poll delete <poll> 
```

| arg  | type                                       | use                |
|------|--------------------------------------------|--------------------|
| poll | [Poll](index.md#autocomplete-option-types) | The poll to delete |

This command deletes a specified a poll. There are some case where legacy polls with a significant number of roles will not be deleted, if that is the case please join the [Support Server](//discord.gg/563mXJBvtP) and ping someone with the `dev` role. Polls cannot be recovered once deleted. 

## Poll Send
{: .d-inline-block }
Slash Command
{: .label .label-green }

```xml
/poll send <poll> 
```

| arg  | type                                       | use              |
|------|--------------------------------------------|------------------|
| poll | [Poll](index.md#autocomplete-option-types) | The poll to send |

This command sends an embed for a specified poll along with a button users can press to launch the poll, when clicked the button will prompt users with a menu they can use to select roles. To update the display of a poll run [``/poll update``](#poll-update).

## Poll Update
{: .d-inline-block }
Slash Command
{: .label .label-green }
Temporary
{: .label .label-yellow }

```xml
/poll update <poll> <name>? <description>? <footer>? <min>? <max>?
```

| arg         | type                                       | use                                                                                                      |
|-------------|--------------------------------------------|----------------------------------------------------------------------------------------------------------|
| poll        | [Poll](index.md#autocomplete-option-types) | The poll to update                                                                                       |
| min         | Int?                                       | The smallest number of roles a user can have from the poll, defaults to 1 if the poll has too few roles. |
| max         | Int?                                       | The largest number of roles a user can have from the poll, defaults to the total number of roles.        |

This command creates a new poll with the specified attributes. To test your poll run [``/poll send``](#poll-send) in a private channel. This command will respond with a [poll update modal](modals.md#creating-and-updating-polls) which you must fill out.
