---
layout: default
title: Pronoun
---

<details open markdown="block">
  <summary>
    Table of contents
  </summary>
  {: .text-delta }
1. TOC
{:toc}
</details>

# Pronouns

CatCore's Pronouns feature is designed to full user customization, including using neo-pronouns not available in many pronoun bots. Because adding hundreds of roles for countless neopronouns used by members was not very feasible CatCore stores user pronouns in a database and allows users to see them by running [slash commands](https://support.discord.com/hc/en-us/articles/1500000368501-Slash-Commands-FAQ).

## Pronoun Add 
{: .d-inline-block }

Slash Command
{: .label .label-green }

```xml
/pronoun add <pronoun> 
```
| arg     | type    | use                                |
|---------|---------|------------------------------------|
| pronoun | Pronoun | The pronoun to add to your profile |

This command adds an already created pronoun to your user profile, to use it type ``/pronoun add`` in the message bar and use the autocompletion to select a pronoun. If you don't see the pronoun you want, run [``/pronoun new``](#pronoun-new) to add it to your profile. If you decide to remove it run [``/pronoun remove``](#pronoun-remove)

![Sample pronoun autocomplete](images/pronoun_add_select.png)

## Pronoun Get
{: .d-inline-block }

Slash Command
{: .label .label-green }

```xml
/pronoun get <user> 
```
| arg  | type | use                           |
|------|------|-------------------------------|
| user | User | The user's pronoun to look up |

This command will lookup a users preferred pronouns and relay them to you. If a user has no pronouns set you could direct them to this documentation!

## Pronoun New
{: .d-inline-block }

Slash Command
{: .label .label-green }

Temporary
{: .label .label-yellow }
```xml
/pronoun new <subject> <object> <possessive-adj> <possessive-pnoun> <reflexive>
```
| arg               | type   | use                                                                 |
|-------------------|--------|---------------------------------------------------------------------|
| subject           | String | The subject pronoun (**they** went to the park)                     |
| object            | String | The object pronoun (I went to the park with **them**)               |
| possessive-adj    | String | The possessive adjective pronoun (the park is near **their** house) |
| possessive-ponoun | String | The possessive pronoun pronoun (that bench is **theirs**)           |
| reflexive         | String | The reflexive pronoun (someone went to the park by **themself**)    |

This command will create a new pronoun you can add to your profile by running [``/pronoun add``](#pronoun-add), this command will be replaced with a [modal](https://github.com/Discord-Net-Labs/Discord.Net-Labs/pull/428) when they are publicly available.

## Pronoun Remove
{: .d-inline-block }

Slash Command
{: .label .label-green }

```xml
/pronoun remove <pronoun> 
```
| arg     | type    | use                                     |
|---------|---------|-----------------------------------------|
| pronoun | pronoun | The pronoun to remove from your profile |

This command will remove a pronoun from your profile, you can always re-add it by running [``/pronoun add``](#pronoun-add).

## Pronoun Try
{: .d-inline-block }

Slash Command
{: .label .label-green }

```xml
/pronoun try <user> 
```
| arg  | type | use                           |
|------|------|-------------------------------|
| user | User | The user's pronoun to look up |

This command will try a pronoun out on you, it will respond in the following format:
> Have you seen @Olivia's latest project? She made a really pretty necklace that looks like Her cats hugging. I wish I spent more time on projects like Her.

## Pronouns
{: .d-inline-block }

Context Menu Command
{: .label .label-blue }

This command can be activated by right clicking a user on the desktop client, otherwise it is no different from [``/pronoun get``](#pronoun-get)
