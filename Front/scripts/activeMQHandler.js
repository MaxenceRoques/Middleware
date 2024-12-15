const client = Stomp.client('ws://localhost:61614');
var isNewItinerary = true;
var subscription = null;

var messageBuffer = [];

client.connectHeaders = {
    login: 'username',
    passcode: 'password'
};

client.onConnect = function (frame) {
    subscription = client.subscribe('/queue/routing', function (message) {
        const receivedMessage = message.body;
        
        handleReceivedMessage(receivedMessage, message);
    },{ ack: 'client' });
};

client.onStompError = function (frame) {
    console.error('STOMP error:', frame);
};

client.onWebSocketError = function (event) {
    console.error('WebSocket error:', event);
};

document.addEventListener('adressSelected', function () {
    isNewItinerary = true;
});

client.connect(client.connectHeaders.login, client.connectHeaders.passcode, client.onConnect, client.onStompError);

function handleReceivedMessage(messageBody, message) {
    messageBuffer.push(message);
    try {
        const step = JSON.parse(messageBody); 
        document.dispatchEvent(new CustomEvent('step', {
            detail: {
                instruction: step.Route.instruction,
                profile: step.Profile,
                isNewItinerary: isNewItinerary
            }
        }));

        if (isNewItinerary) {
            document.dispatchEvent(new CustomEvent('firstStep', {
                detail: {
                    firstStep: step.Route.instruction
                }
            }));
        }

        isNewItinerary = false;
    } catch (error) {
        console.error('Error parsing the received message:', error);
    }
}

export function acknowledgeAllMessages() {
    messageBuffer.forEach((message) => {
        message.ack();
    });
    messageBuffer = [];
}
