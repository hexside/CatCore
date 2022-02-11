---
layout: default
title: Role
parent: Poll
---


# Poll Role
A poll is a menu created by CatCore that lets your members assign roles to themselves. Members must have the ``Manage Roles`` permission to create and edit polls. 

## Poll Role Add
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

This command will add a role to a poll. If the role is already in the poll it cannot be added again. To update roles run [``/poll remove-role``](#poll-role-remove) and re-add it.

## Poll Role Remove
{: .d-inline-block }
Slash Command
{: .label .label-green }

```xml
/poll add-role <poll> <role>
```

| arg  | type                                       | use                         |
|------|--------------------------------------------|-----------------------------|
| poll | [Poll](index.md#autocomplete-option-types) | The poll to add the role to |
| role | Role                                       | The role to add to the poll |

This command will remove a role from a poll. To re-add the role run [``/poll add-role``](#poll-role-add).

