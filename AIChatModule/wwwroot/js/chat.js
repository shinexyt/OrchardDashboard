class ChatApp {
    constructor() {
        this.messages = [];
        this.messageInput = document.getElementById('messageInput');
        this.sendButton = document.getElementById('sendButton');
        this.chatMessages = document.getElementById('chatMessages');
        this.isStreaming = false;
        
        this.init();
    }
    
    init() {
        this.sendButton.addEventListener('click', () => this.sendMessage());
        this.messageInput.addEventListener('keydown', (e) => {
            if (e.key === 'Enter' && !e.shiftKey) {
                e.preventDefault();
                this.sendMessage();
            }
        });
        
        // Auto-resize textarea
        this.messageInput.addEventListener('input', () => {
            this.messageInput.style.height = 'auto';
            this.messageInput.style.height = this.messageInput.scrollHeight + 'px';
        });
    }
    
    async sendMessage() {
        const content = this.messageInput.value.trim();
        if (!content || this.isStreaming) return;
        
        // Clear welcome message on first message
        const welcomeMessage = this.chatMessages.querySelector('.welcome-message');
        if (welcomeMessage) {
            welcomeMessage.remove();
        }
        
        // Add user message
        this.addMessage('user', content);
        this.messages.push({ role: 'user', content: content });
        
        // Clear input
        this.messageInput.value = '';
        this.messageInput.style.height = 'auto';
        
        // Disable input while streaming
        this.setInputState(false);
        
        // Create assistant message placeholder
        const assistantMessageDiv = this.createMessageElement('assistant', '');
        const contentDiv = assistantMessageDiv.querySelector('.message-content');
        
        try {
            await this.streamResponse(contentDiv);
        } catch (error) {
            this.showError(error.message);
            assistantMessageDiv.remove();
        } finally {
            this.setInputState(true);
            this.messageInput.focus();
        }
    }
    
    async streamResponse(contentDiv) {
        this.isStreaming = true;
        let fullResponse = '';
        
        const response = await fetch('/chat/stream', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ messages: this.messages })
        });
        
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        const reader = response.body.getReader();
        const decoder = new TextDecoder();
        
        while (true) {
            const { done, value } = await reader.read();
            if (done) break;
            
            const chunk = decoder.decode(value);
            const lines = chunk.split('\n');
            
            for (const line of lines) {
                if (line.startsWith('data: ')) {
                    const data = line.slice(6);
                    
                    if (data === '[DONE]') {
                        this.isStreaming = false;
                        this.messages.push({ role: 'assistant', content: fullResponse });
                        return;
                    }
                    
                    try {
                        const parsed = JSON.parse(data);
                        if (parsed.error) {
                            throw new Error(parsed.error);
                        }
                        if (parsed.content) {
                            fullResponse += parsed.content;
                            contentDiv.textContent = fullResponse;
                            this.scrollToBottom();
                        }
                    } catch (e) {
                        // Ignore JSON parse errors for partial data
                        if (!e.message.includes('Unexpected')) {
                            throw e;
                        }
                    }
                }
            }
        }
        
        this.isStreaming = false;
    }
    
    addMessage(role, content) {
        const messageDiv = this.createMessageElement(role, content);
        this.scrollToBottom();
    }
    
    createMessageElement(role, content) {
        const messageDiv = document.createElement('div');
        messageDiv.className = `message ${role}`;
        
        const avatar = document.createElement('div');
        avatar.className = 'message-avatar';
        avatar.textContent = role === 'user' ? 'U' : 'AI';
        
        const contentDiv = document.createElement('div');
        contentDiv.className = 'message-content';
        contentDiv.textContent = content;
        
        messageDiv.appendChild(avatar);
        messageDiv.appendChild(contentDiv);
        this.chatMessages.appendChild(messageDiv);
        
        return messageDiv;
    }
    
    setInputState(enabled) {
        this.messageInput.disabled = !enabled;
        this.sendButton.disabled = !enabled;
    }
    
    showError(message) {
        const errorDiv = document.createElement('div');
        errorDiv.className = 'error-message';
        errorDiv.textContent = `Error: ${message}`;
        this.chatMessages.appendChild(errorDiv);
        
        setTimeout(() => {
            errorDiv.remove();
        }, 5000);
        
        this.scrollToBottom();
    }
    
    scrollToBottom() {
        this.chatMessages.scrollTop = this.chatMessages.scrollHeight;
    }
}

// Initialize chat app when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    new ChatApp();
});
