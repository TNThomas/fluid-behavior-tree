﻿using Adnc.FluidBT.TaskParents;
using Adnc.FluidBT.Tasks;
using Adnc.FluidBT.Tasks.Actions;
using NSubstitute;
using NUnit.Framework;

namespace Adnc.FluidBT.Testing {
    public class TaskParentTest {
        private TaskParentExample taskParent;

        [SetUp]
        public void SetTaskParent () {
            taskParent = new TaskParentExample();
        }

        public class TaskParentExample : TaskParentBase {
            public int childCount = -1;

            protected override int MaxChildren => childCount;
        }

        public class TaskExample : ActionBase {
            protected override TaskStatus OnUpdate () {
                return TaskStatus.Success;
            }
        }

        public class ResetMethod : TaskParentTest {
            [Test]
            public void Runs_reset_on_the_child_task () {
                var task = Substitute.For<ITask>();
                task.Enabled.Returns(true);
                taskParent.AddChild(task);

                taskParent.Reset();

                task.Received().Reset();
            }

            [Test]
            public void Runs_reset_on_multiple_child_tasks () {
                var taskA = Substitute.For<ITask>();
                var taskB = Substitute.For<ITask>();
                taskParent.AddChild(taskA);
                taskParent.AddChild(taskB);

                taskParent.Reset();

                taskA.Received().Reset();
                taskB.Received().Reset();
            }

            [Test]
            public void Does_not_fail_if_child_task_is_missing () {
                taskParent.Reset();
            }
        }
        
        public class AddChildMethod : TaskParentTest {
            [Test]
            public void Adds_a_child () {
                taskParent.AddChild(new TaskExample());
                
                Assert.AreEqual(1, taskParent.children.Count);
            }
            
            [Test]
            public void Adds_two_children () {
                taskParent.AddChild(new TaskExample());
                taskParent.AddChild(new TaskExample());

                Assert.AreEqual(2, taskParent.children.Count);
            }
            
            [Test]
            public void Ignores_overflowing_children () {
                taskParent.childCount = 1;
                
                taskParent.AddChild(new TaskExample());
                taskParent.AddChild(new TaskExample());

                Assert.AreEqual(1, taskParent.children.Count);
            }
        }
    }
}