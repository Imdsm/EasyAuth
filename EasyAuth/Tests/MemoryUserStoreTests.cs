﻿using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EasyAuth;
using EasyAuth.Storage;
using System.Collections.Generic;

namespace EasyAuth.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class MemoryUserStoreTests
    {
        private MemoryUserStore userStore;

        [TestInitialize]
        public void TestInitialize()
        {
            userStore = MemoryUserStore.Instance;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            userStore.Reset();
        }

        #region AddUser tests
        [TestMethod]
        public void MemoryUserStore_AddUser_GivenNewUser_UserAdded()
        {
            string username = "testuser", password = "testpass";
            userStore.AddUser(username, password);

            User user = userStore.GetUserByUsername(username);
            var actual = user.Username;
            var expected = username;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(EasyAuth.UserAlreadyExistsException))]
        public void MemoryUserStore_AddUser_GivenAlreadyExistingUsername_ThrowsException()
        {
            string username = "testuser", password = "testpass";
            userStore.AddUser(username, password);

            userStore.AddUser(username, password);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void MemoryUserStore_AddUser_GivenEmptyFirstArgument_ThrowsException()
        {
            userStore.AddUser("", "password");
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void MemoryUserStore_AddUser_GivenEmptySecondArgument_ThrowsException()
        {
            userStore.AddUser("username", "");
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void MemoryUserStore_AddUser_GivenNullFirstArgument_ThrowsException()
        {
            userStore.AddUser(null, "password");
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void MemoryUserStore_AddUser_GivenNullSecondArgument_ThrowsException()
        {
            userStore.AddUser("username", null);
        }
        #endregion

        #region DeleteUser tests
        [TestMethod]
        public void MemoryUserStore_DeleteUser_GivenExistingUserId_UserDeleted()
        {
            string username = "testuser", password = "testpass";
            userStore.AddUser(username, password);

            userStore.DeleteUserById(userStore.GetUserByUsername(username).UserId);
            var actual = userStore.UserExistsByUsername(username);

            Assert.IsFalse(actual);
        }

        [TestMethod]
        [ExpectedException(typeof(EasyAuth.UserDoesNotExistException))]
        public void MemoryUserStore_DeleteUser_GivenNonExistantUserId_ThrowsException()
        {
            userStore.DeleteUserById(395);
        }
        #endregion

        #region UpdateUserById tests
        [TestMethod]
        public void MemoryUserStore_UpdateUserById_GivenExistingUser_UserUpdated()
        {
            string username = "testuser", password = "testpass", newUsername = "newusername";
            userStore.AddUser(username, password);
            User user = userStore.GetUserByUsername(username);
            
            user.Username = newUsername;
            userStore.UpdateUserById(user.UserId, user);
            var actual = userStore.UserExistsByUsername(newUsername);

            Assert.IsTrue(actual);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void MemoryUserStore_UpdateUserById_GivenNullSecondArgument_ThrowsException()
        {
            string username = "testuser", password = "testpass";
            userStore.AddUser(username, password);
            User user = userStore.GetUserByUsername(username);

            userStore.UpdateUserById(user.UserId, null);
        }

        [TestMethod]
        [ExpectedException(typeof(EasyAuth.UserDoesNotExistException))]
        public void MemoryUserStore_UpdateUserById_GivenNonExistantUserId_ThrowsException()
        {
            string username = "testuser", password = "testpass";
            userStore.AddUser(username, password);
            User user = userStore.GetUserByUsername(username);

            userStore.UpdateUserById(26, user);
        }

        [TestMethod]
        [ExpectedException(typeof(EasyAuth.UserIdDoesNotMatchUserObjectIdException))]
        public void MemoryUserStore_UpdateUserById_GivenUserIdThatDoesNotMatchUserObjectId_ThrowsException()
        {
            userStore.AddUser("user1", "password");
            userStore.AddUser("user2", "password");
            User user1 = userStore.GetUserByUsername("user1");
            User user2 = userStore.GetUserByUsername("user2");

            userStore.UpdateUserById(user1.UserId, user2);
        }

        [TestMethod]
        public void MemoryUserStore_UpdateUserById_UserChangedWithoutUpdate_NotAffectedByUnrelatedUpdate()
        {
            string nameA = "userA", nameB = "userB";
            userStore.AddUser(nameA, "passA");
            userStore.AddUser(nameB, "passB");
            User userA = userStore.GetUserByUsername(nameA);
            User userB = userStore.GetUserByUsername(nameB);

            userA.Username = "changedA";
            userB.Username = "changedB";
            userStore.UpdateUserById(userB.UserId, userB);
            var actual = userStore.UserExistsByUsername(nameA);

            Assert.IsTrue(actual);
        }
        #endregion

        #region UserExistsById tests
        [TestMethod]
        public void MemoryUserStore_UserExistsById_GivenExistingUserId_ReturnsTrue()
        {
            userStore.AddUser("user1", "password");            
            User user1 = userStore.GetUserByUsername("user1");

            var actual = userStore.UserExistsById(user1.UserId);

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void MemoryUserStore_UserExistsById_GivenNonExistantUserId_ReturnsFalse()
        {
            var actual = userStore.UserExistsById(236);

            Assert.IsFalse(actual);
        }
        #endregion

        #region UserExistsByUsername tests
        [TestMethod]
        public void MemoryUserStore_UserExistsByUsername_GivenExistingUsername_ReturnsTrue()
        {
            userStore.AddUser("user1", "password");

            var actual = userStore.UserExistsByUsername("user1");

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void MemoryUserStore_UserExistsByUsername_GivenNonExistantUserId_ReturnsFalse()
        {
            var actual = userStore.UserExistsByUsername("doesnotexist");

            Assert.IsFalse(actual);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void MemoryUserStore_UserExistsByUsername_GivenNull_ThrowsException()
        {
            userStore.UserExistsByUsername(null);
        }
        #endregion

        #region GetUserById tests
        [TestMethod]
        public void MemoryUserStore_GetUserById_GivenExistingUserId_ReturnsCorrectUser()
        {
            string username = "user1";
            userStore.AddUser(username, "password");
            User user1 = userStore.GetUserByUsername(username);

            User user = userStore.GetUserById(user1.UserId);
            var expected = username;
            var actual = user.Username;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentException))]
        public void MemoryUserStore_GetUserById_GivenNegativeId_ThrowsException()
        {
            userStore.GetUserById(-20);
        }

        [TestMethod]
        [ExpectedException(typeof(EasyAuth.UserDoesNotExistException))]
        public void MemoryUserStore_GetUserById_GivenNonExistantUserId_ThrowsException()
        {
            userStore.GetUserById(12);
        }

        [TestMethod]
        public void MemoryUserStore_GetUserById_UserChangedWithoutUpdate_StoredUserNotAffected()
        {
            string username = "user1";
            userStore.AddUser(username, "password");
            int userId = userStore.GetUserByUsername(username).UserId;

            User userNotUpdated = userStore.GetUserById(userId);
            userNotUpdated.Username = "bogususername";
            User userOriginal = userStore.GetUserById(userId);
            var expected = username;
            var actual = userOriginal.Username;

            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region GetUserByUsername tests
        [TestMethod]
        public void MemoryUserStore_GetUserByUsername_GivenExistingUsername_ReturnsCorrectUser()
        {
            string username = "user1";
            userStore.AddUser(username, "password");
            
            User user = userStore.GetUserByUsername(username);
            var expected = username;
            var actual = user.Username;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void MemoryUserStore_GetUserByUsername_GivenNullArgument_ThrowsException()
        {
            userStore.GetUserByUsername(null);
        }

        [TestMethod]
        [ExpectedException(typeof(EasyAuth.UserDoesNotExistException))]
        public void MemoryUserStore_GetUserByUsername_GivenNonExistantUsername_ThrowsException()
        {
            userStore.GetUserByUsername("NotAValidUser");
        }

        [TestMethod]
        public void MemoryUserStore_GetUserByUsername_UserChangedWithoutUpdate_StoredUserNotAffected()
        {
            string username = "user1";
            userStore.AddUser(username, "password");

            User userNotUpdated = userStore.GetUserByUsername(username);
            userNotUpdated.Username = "bogususername";
            User userOriginal = userStore.GetUserByUsername(username);
            var expected = username;
            var actual = userOriginal.Username;

            Assert.AreEqual(expected, actual);
        }
        #endregion

        #region GetAllUsers tests
        [TestMethod]
        public void MemoryUserStore_GetAllUsers_MultipleUsersAdded_ReturnsCorrectAmount()
        {
            userStore.AddUser("user1", "pass");
            userStore.AddUser("user2", "pass");
            userStore.AddUser("user3", "pass");

            List<User> allUsers = userStore.GetAllUsers();
            var expected = 3;
            var actual = allUsers.Count;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void MemoryUserStore_GetAllUsers_MultipleUsersAdded_ReturnsCorrectUsers()
        {
            bool containsUser1 = false, containsUser2 = false;
            userStore.AddUser("user1", "pass");
            userStore.AddUser("user2", "pass");
            User user1 = userStore.GetUserByUsername("user1");
            User user2 = userStore.GetUserByUsername("user2");

            List<User> allUsers = userStore.GetAllUsers();
            foreach (User u in allUsers)
            {
                if (u.Username == user1.Username) containsUser1 = true;
                if (u.Username == user2.Username) containsUser2 = true;
            }

            var actual = (containsUser1 && containsUser2);

            Assert.IsTrue(actual);
        }
        #endregion
    }
}
