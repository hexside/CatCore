---
layout: default
title: Custom Ids
parent: DevTools
---

<details open markdown="block">
  <summary>
    Table of contents
  </summary>
  {: .text-delta }
- TOC
{:toc}
</details>

# Custom Ids

## Mail

| type   | name                           | description                                                    |
|--------|--------------------------------|----------------------------------------------------------------|
| button | ``user.notifications.dismiss`` | Marks all user mail with the ``Unread`` flag as ``Suppressed`` |

## Poll

| type   | name                   | description                           | param 1 (or value)                               | param 2                                       | param 3                                      |
|--------|------------------------|---------------------------------------|--------------------------------------------------|-----------------------------------------------|----------------------------------------------|
| modal  | ``poll.create:*,*;``   | Creates a new poll                    | ``int?`` The smallest number of allowed roles    | ``int?`` The largest number of allowed roles  | -                                            |
| modal  | ``poll.*.update:*,*;`` | Modifies an already created poll      | ``int`` The poll's ID                            | ``int?`` The smallest number of allowed roles | ``int?`` The largest number of allowed roles |
| button | ``poll.*.launch``      | Launches the menu on an embedded poll | ``int`` The poll's ID                            | -                                             | -                                            |
| select | ``poll.*.result``      | Selects user roles from a pol         | ``list<ulong>`` The role Id's chosen by the user | The poll's ID                                 | -                                            |

## AutoMod

| type  | name                             | description                               | param 1 (or value0)     | param 2                                    |
|-------|----------------------------------|-------------------------------------------|-------------------------|--------------------------------------------|
| modal | ``automod.filter.*.setRegex:*;`` | Modifies the regex for an automod filter. | ``int`` The filter's ID | ``(int)bool`` Should the regex be escaped? |
