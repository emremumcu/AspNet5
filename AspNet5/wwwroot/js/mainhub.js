﻿"use strict";

var sendButton = document.getElementById("sendButton");

sendButton.disabled = true;

sendButton.addEventListener("click", function (event) {
    var user = "";
    var message = document.getElementById("messageInput").value;

    connection
        .invoke("BroadcastMessage", message)
        .catch(function (err) { logger(err.toString()); });

    event.preventDefault();
});



var connection = new signalR.HubConnectionBuilder().withUrl("/mainhub").build();

connection
    .start()
    .then(function () { sendButton.disabled = false; })
    .catch(function (err) { logger(err.toString()); });

connection.on("ReceiveMessage", function (user, message)
{
    showMessage(user, message);
});

connection.on("UserConnected", function (user) {
    addUser(user);
});

connection.on("UserDisconnected", function (user) {
    removeUser(user);
});

connection.on("UserList", function (userlist) {
    allUsers(userlist);
});


/*
 
 var editor = document.getElementById("editor");

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/textHub")
    .build();
connection.start().catch(err => console.error(err));

connection.on("ReceiveText", (text) => {
    editor.value = text;
    editor.focus();
    editor.setSelectionRange(editor.value.length, editor.value.length);
});

function change() {
    connection.invoke("BroadcastText", editor.value).catch(err => console.error(err));
}

editor.style.display = "none";
var group = document.getElementById("group");
function join() {
    connection.invoke("JoinGroup", group.value).catch(err => console.error(err));
    editor.style.display = "initial";
}
 

"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {

    console.log(user, message);

    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = user + " says " + msg;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});

connection.start().then(function ()
{
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event)
{
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});
 
 
 
 */

/*
 "use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/mainhub").build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = user + " says " + msg;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});







    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/chathub")
        .configureLogging(signalR.LogLevel.Trace)
        .build();

    connection.on("ReceiveMessage", (user, message) => {
        const li = document.createElement("li");
    li.textContent = user + " says " + message;
    document.getElementById("messagesList").appendChild(li);
});

connection.start().catch(err => console.error(err.toString()));

    document.getElementById("sendButton").addEventListener("click", event => {
        const user = document.getElementById("userInput").value;
    const message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", user, message).catch(err => console.error(err.toString()));
    event.preventDefault();
});





"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/mainhub").build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message)
{
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = user + " says " + msg;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});







"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/mainhub", {
    accessTokenFactory: () => "MyTokenGoesHere"
}).build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message)
{
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = user + " says " + msg;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});















"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/pollHub").build();
var chartBlock = '\u25A3'; //(U+25A3) is "▣"

connection.on("ReceiveMessage", function (user, message, myCaptainId, myCaptainVal) {
    // alert("myCaptainId=" + myCaptainId + ",myCaptainVal=" + myCaptainVal);
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    // var encodedMsg = user + " says " + msg;
    var pollResultMsg = user + " voted for '" + myCaptainVal + "'.";

    // var liMessage = document.createElement("li");
    // liMessage.textContent = encodedMsg;
    // document.getElementById("messagesList").appendChild(liMessage);

    var ulPoll = document.getElementById("messagesList");
    var liPollResult = document.createElement("li");
    liPollResult.textContent = pollResultMsg;

    // append to top
    ulPoll.insertBefore(liPollResult, ulPoll.childNodes[0]);

    // append to end
    // document.getElementById("messagesList").appendChild(liPollResult);

    // append to chart block
    document.getElementById(myCaptainId + 'Block').innerHTML += chartBlock;
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = ""; //document.getElementById("messageInput").value;
    //var myCaptain = document.getElementById("myCaptain").value;

    if (!user) {
        user = "[anonymous]";
    }

    if ($('input:radio[name=myCaptain]').is(':checked')) {
        var myCaptainId = $('input[name=myCaptain]:checked').attr('id');
        var myCaptainVal = $('input[name=myCaptain]:checked').val();
        connection.invoke("SendMessage", user, message, myCaptainId, myCaptainVal).catch(function (err) {
            return console.error(err.toString());
        });
    } else {
        return console.log("No captain selected.");
    }

    event.preventDefault();
});














 
 */


/*
 <!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title></title>

    <link type="text/css" rel="stylesheet" href="Css/ChatStyle.css" />
    <link rel="stylesheet" href="/Css/JQueryUI/themes/base/jquery.ui.all.css">





    <!--Script references. -->

      <!--Reference the jQuery library. -->
    <script src="/Scripts/jquery-1.8.2.min.js"></script>

    <script src="/Scripts/ui/jquery.ui.core.js"></script>
    <script src="/Scripts/ui/jquery.ui.widget.js"></script>
    <script src="/Scripts/ui/jquery.ui.mouse.js"></script>
    <script src="/Scripts/ui/jquery.ui.draggable.js"></script>
    <script src="/Scripts/ui/jquery.ui.resizable.js"></script>



    <!--Reference the SignalR library. -->
    <script src="/Scripts/jquery.signalR-1.0.0.js"></script>

    <!--Reference the autogenerated SignalR hub script. -->
    <script src="/signalr/hubs"></script>

    <script type="text/javascript">

        $(function () {

            setScreen(false);

            // Declare a proxy to reference the hub.
            var chatHub = $.connection.chatHub;

            registerClientMethods(chatHub);

            // Start Hub
            $.connection.hub.start().done(function () {

                registerEvents(chatHub)

            });

        });

        function setScreen(isLogin) {

            if (!isLogin) {

                $("#divChat").hide();
                $("#divLogin").show();
            }
            else {

                $("#divChat").show();
                $("#divLogin").hide();
            }

        }

        function registerEvents(chatHub) {

            $("#btnStartChat").click(function () {

                var name = $("#txtNickName").val();
                if (name.length > 0) {
                    chatHub.server.connect(name);
                }
                else {
                    alert("Please enter name");
                }

            });


            $('#btnSendMsg').click(function () {

                var msg = $("#txtMessage").val();
                if (msg.length > 0) {

                    var userName = $('#hdUserName').val();
                    chatHub.server.sendMessageToAll(userName, msg);
                    $("#txtMessage").val('');
                }
            });


            $("#txtNickName").keypress(function (e) {
                if (e.which == 13) {
                    $("#btnStartChat").click();
                }
            });

            $("#txtMessage").keypress(function (e) {
                if (e.which == 13) {
                    $('#btnSendMsg').click();
                }
            });


        }

        function registerClientMethods(chatHub) {

            // Calls when user successfully logged in
            chatHub.client.onConnected = function (id, userName, allUsers, messages) {

                setScreen(true);

                $('#hdId').val(id);
                $('#hdUserName').val(userName);
                $('#spanUser').html(userName);

                // Add All Users
                for (i = 0; i < allUsers.length; i++) {

                    AddUser(chatHub, allUsers[i].ConnectionId, allUsers[i].UserName);
                }

                // Add Existing Messages
                for (i = 0; i < messages.length; i++) {

                    AddMessage(messages[i].UserName, messages[i].Message);
                }


            }

            // On New User Connected
            chatHub.client.onNewUserConnected = function (id, name) {

                AddUser(chatHub, id, name);
            }


            // On User Disconnected
            chatHub.client.onUserDisconnected = function (id, userName) {

                $('#' + id).remove();

                var ctrId = 'private_' + id;
                $('#' + ctrId).remove();


                var disc = $('<div class="disconnect">"' + userName + '" logged off.</div>');

                $(disc).hide();
                $('#divusers').prepend(disc);
                $(disc).fadeIn(200).delay(2000).fadeOut(200);

            }

            chatHub.client.messageReceived = function (userName, message) {

                AddMessage(userName, message);
            }


            chatHub.client.sendPrivateMessage = function (windowId, fromUserName, message) {

                var ctrId = 'private_' + windowId;


                if ($('#' + ctrId).length == 0) {

                    createPrivateChatWindow(chatHub, windowId, ctrId, fromUserName);

                }

                $('#' + ctrId).find('#divMessage').append('<div class="message"><span class="userName">' + fromUserName + '</span>: ' + message + '</div>');

                // set scrollbar
                var height = $('#' + ctrId).find('#divMessage')[0].scrollHeight;
                $('#' + ctrId).find('#divMessage').scrollTop(height);

            }

        }

        function AddUser(chatHub, id, name) {

            var userId = $('#hdId').val();

            var code = "";

            if (userId == id) {

                code = $('<div class="loginUser">' + name + "</div>");

            }
            else {

                code = $('<a id="' + id + '" class="user" >' + name + '<a>');

                $(code).dblclick(function () {

                    var id = $(this).attr('id');

                    if (userId != id)
                        OpenPrivateChatWindow(chatHub, id, name);

                });
            }

            $("#divusers").append(code);

        }

        function AddMessage(userName, message) {
            $('#divChatWindow').append('<div class="message"><span class="userName">' + userName + '</span>: ' + message + '</div>');

            var height = $('#divChatWindow')[0].scrollHeight;
            $('#divChatWindow').scrollTop(height);
        }

        function OpenPrivateChatWindow(chatHub, id, userName) {

            var ctrId = 'private_' + id;

            if ($('#' + ctrId).length > 0) return;

            createPrivateChatWindow(chatHub, id, ctrId, userName);

        }

        function createPrivateChatWindow(chatHub, userId, ctrId, userName) {

            var div = '<div id="' + ctrId + '" class="ui-widget-content draggable" rel="0">' +
                       '<div class="header">' +
                          '<div  style="float:right;">' +
                              '<img id="imgDelete"  style="cursor:pointer;" src="/Images/delete.png"/>' +
                           '</div>' +

                           '<span class="selText" rel="0">' + userName + '</span>' +
                       '</div>' +
                       '<div id="divMessage" class="messageArea">' +

                       '</div>' +
                       '<div class="buttonBar">' +
                          '<input id="txtPrivateMessage" class="msgText" type="text"   />' +
                          '<input id="btnSendMessage" class="submitButton button" type="button" value="Send"   />' +
                       '</div>' +
                    '</div>';

            var $div = $(div);

            // DELETE BUTTON IMAGE
            $div.find('#imgDelete').click(function () {
                $('#' + ctrId).remove();
            });

            // Send Button event
            $div.find("#btnSendMessage").click(function () {

                $textBox = $div.find("#txtPrivateMessage");
                var msg = $textBox.val();
                if (msg.length > 0) {

                    chatHub.server.sendPrivateMessage(userId, msg);
                    $textBox.val('');
                }
            });

            // Text Box event
            $div.find("#txtPrivateMessage").keypress(function (e) {
                if (e.which == 13) {
                    $div.find("#btnSendMessage").click();
                }
            });

            AddDivToContainer($div);

        }

        function AddDivToContainer($div) {
            $('#divContainer').prepend($div);

            $div.draggable({

                handle: ".header",
                stop: function () {

                }
            });

            ////$div.resizable({
            ////    stop: function () {

            ////    }
            ////});

        }

    </script>

</head>
<body>
    <div id="header">
        SignalR Chat Room
    </div>
    <br />
    <br />
    <br />

    <div id="divContainer">
        <div id="divLogin" class="login">
            <div>
                Your Name:<br />
            <input id="txtNickName" type="text" class="textBox" />
            </div>
            <div id="divButton">
                <input id="btnStartChat" type="button" class="submitButton" value="Start Chat" />
            </div>
        </div>

        <div id="divChat" class="chatRoom">
            <div class="title">
                Welcome to Chat Room [<span id='spanUser'></span>]

            </div>
            <div class="content">
                <div id="divChatWindow" class="chatWindow">
                </div>
                <div id="divusers" class="users">
                </div>
            </div>
            <div class="messageBar">
                <input class="textbox" type="text" id="txtMessage" />
                <input id="btnSendMsg" type="button" value="Send" class="submitButton" />
            </div>
        </div>

        <input id="hdId" type="hidden" />
        <input id="hdUserName" type="hidden" />
    </div>

</body>
</html>

 */