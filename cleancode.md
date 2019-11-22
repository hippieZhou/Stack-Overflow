# 重构：改善即有代码的的设计

## 学习笔记

- 是需求的变化使得重构变得必要，如果一段代码能正常功能，并且不会被修改，那么完全可以不去重构它；
- 如果确实需要理解它的原理，并且觉得理解起来费劲，那么就需要改进代码；
- 重构前，先检查自己是否有一套可靠的测试集，这些测试必须有自我检验能力；
- 编程时要遵循营地法则：保证你离开时的代码库一定比来时更健康；
- 好代码的检验标准就是人们是否轻而易举地修改它；

## 重构的原则

- 为何重构
  - 改进软件的设计
  - 使软件更容易理解
  - 帮助找到 bug
  - 提高编程速度
- 何时重构
  - 预备性重构：让添加新功能更容易
  - 帮助理解的重构：使代码更易懂
  - 捡垃圾式重构
  - 有计划的重构和见机行事的重构
  - 长期重构
  - 复审代码时重构
- 代码的坏味道
  - 神秘命名 (Mysterious Name)
  - 重复代码 (Duplicated Code)
  - 过长函数 (Long Function)
  - 过长参数列表 (Long Parameter List)
  - 全局数据 (Global Data)
  - 可变数据 (Mutable Data)
  - 发散式变化 (Divergent Change)
  - 散弹式修改 (Shotgun Surgery)
  - 依恋情结 (Feature Envy)
  - 数据泥团 (Data Clumps)
  - 基本类型偏执 (Primitive Obsession)
  - 重复的 Switch (Repeated Switches)
  - 循环语句 (Loops)
  - 冗赘的元素 (Lazy Element)
  - 夸夸其谈通用性 (Speculative Generality)
  - 临时字段 (Temporary Field)
  - 过长的消息链 (Message Chains)
  - 中间人 (Middle Man)
  - 内幕交易 (Insider Trading)
  - 过大的类 (Large Class)
  - 异曲同工的类 (Alternative Classes with Different Interfaces)
  - 纯数据类 (Data Class)
  - 被拒绝的遗赠 (Refused Bequest)
  - 注释 (Comments)
- 构筑测试体系