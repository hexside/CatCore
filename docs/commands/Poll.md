---
layout: default
title: Poll
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

# Poll

A poll is a menu created by CatCore that lets your members assign roles to themselves using menus created by your admins. In order to use polls members must have the ``Manage Roles`` permission.

## Poll Add-Role
{: .d-inline-block }
Slash Command
{: .label .label-green }

```xml
/poll add-role <poll> <role> <description>?
```

| arg         | type                                       | use                                                              |
|-------------|--------------------------------------------|------------------------------------------------------------------|
| poll        | [Poll](index.md#autocomplete-option-types) | The poll to add the role to                                      |
| role        | Role                                       | The role to add to the poll                                      |
| description | String?                                    | The role's description in the menu (defaults to the role's name) |

This command will add a role to a poll. If the role is already in the poll it cannot be added again. To update roles run [``/poll remove-role``](#poll-remove-role) and re-add it.

## Poll New 
{: .d-inline-block }
Slash Command
{: .label .label-green }
Temporary
{: .label .label-yellow }

```xml
/poll add-role <name> <description>? <footer>? <min>? <max>?
```

| arg         | type    | use                                                                                                      |
|-------------|---------|----------------------------------------------------------------------------------------------------------|
| name        | String  | The poll's title (The text on top of it's embed)                                                         |
| description | String? | The poll's description (The text under it's title)                                                       |
| footer      | String? | The poll's footer (The text at the bottom if it's embed)                                                 |
| min         | Int?    | The smallest number of roles a user can have from the poll, defaults to 1 if the poll has too few roles. |
| max         | Int?    | The largest number of roles a user can have from the poll, defaults to the total number of roles.        |

This command creates a new poll with the specified attributes, by default polls have no roles, in order to add them run [``/poll add-role``](#poll-add-role). To test your poll run [``/poll send``](#poll-send) in a private channel. To modify a poll after creating it run [``/poll update``](#poll-update).

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

## Poll Remove-Role
{: .d-inline-block }
Slash Command
{: .label .label-green }

```xml
/poll add-role <poll> <role> <description>?
```

| arg  | type                                       | use                         |
|------|--------------------------------------------|-----------------------------|
| poll | [Poll](index.md#autocomplete-option-types) | The poll to add the role to |
| role | Role                                       | The role to add to the poll |

This command will remove a role from a poll. To re-add the role run [``/poll remove-role``](#poll-remove-role).

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
| name        | String?                                    | The poll's title (The text on top of it's embed)                                                         |
| description | String?                                    | The poll's description (The text under it's title)                                                       |
| footer      | String?                                    | The poll's footer (The text at the bottom if it's embed)                                                 |
| min         | Int?                                       | The smallest number of roles a user can have from the poll, defaults to 1 if the poll has too few roles. |
| max         | Int?                                       | The largest number of roles a user can have from the poll, defaults to the total number of roles.        |

This command creates a new poll with the specified attributes, by default polls have no roles, in order to add them run [``/poll add-role``](#poll-add-role). To test your poll run [``/poll send``](#poll-send) in a private channel. 
