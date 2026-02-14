class ChatApp {
    constructor() {
        this.messages = [];
        this.isProcessing = false;
        
        this.chatMessages = document.getElementById('chatMessages');
        this.userInput = document.getElementById('userInput');
        this.sendButton = document.getElementById('sendButton');
        
        this.init();
    }
    
    init() {
        // Auto-resize textarea
        this.userInput.addEventListener('input', () => {
            this.userInput.style.height = 'auto';
            this.userInput.style.height = this.userInput.scrollHeight + 'px';
        });
        
        // Send on Enter (Shift+Enter for new line)
        this.userInput.addEventListener('keydown', (e) => {
            if (e.key === 'Enter' && !e.shiftKey) {
                e.preventDefault();
                this.sendMessage();
            }
        });
        
        // Send button click
        this.sendButton.addEventListener('click', () => this.sendMessage());
    }
    
    async sendMessage() {
        const content = this.userInput.value.trim();
        
        if (!content || this.isProcessing) {
            return;
        }
        
        // Clear input
        this.userInput.value = '';
        this.userInput.style.height = 'auto';
        
        // Add user message
        this.addMessage('user', content);
        this.messages.push({ role: 'user', content });
        
        // Start processing
        this.isProcessing = true;
        this.sendButton.disabled = true;
        
        // Remove welcome message if exists
        const welcomeMsg = this.chatMessages.querySelector('.welcome-message');
        if (welcomeMsg) {
            welcomeMsg.remove();
        }
        
        // Add assistant message placeholder with typing indicator
        const assistantMessageDiv = this.createMessageElement('assistant', '');
        const typingIndicator = document.createElement('div');
        typingIndicator.className = 'typing-indicator';
        typingIndicator.innerHTML = '<span></span><span></span><span></span>';
        assistantMessageDiv.querySelector('.message-content').appendChild(typingIndicator);
        this.chatMessages.appendChild(assistantMessageDiv);
        this.scrollToBottom();
        
        try {
            await this.streamResponse(assistantMessageDiv);
        } catch (error) {
            console.error('Error:', error);
            assistantMessageDiv.querySelector('.message-content').innerHTML = 
                `<p style="color: #ef4444;">Error: ${error.message}</p>`;
        } finally {
            this.isProcessing = false;
            this.sendButton.disabled = false;
            this.userInput.focus();
        }
    }
    
    async streamResponse(messageDiv) {
        const contentDiv = messageDiv.querySelector('.message-content');
        let fullContent = '';
        
        // Remove typing indicator
        const typingIndicator = contentDiv.querySelector('.typing-indicator');
        if (typingIndicator) {
            typingIndicator.remove();
        }
        
        const response = await fetch('/chat/api/stream', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                messages: this.messages,
                model: 'openai/gpt-3.5-turbo'
            })
        });
        
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        const reader = response.body.getReader();
        const decoder = new TextDecoder();
        
        while (true) {
            const { done, value } = await reader.read();
            
            if (done) {
                break;
            }
            
            const chunk = decoder.decode(value);
            const lines = chunk.split('\n');
            
            for (const line of lines) {
                if (line.startsWith('data: ')) {
                    const data = line.substring(6);
                    
                    try {
                        const parsed = JSON.parse(data);
                        
                        if (parsed.error) {
                            throw new Error(parsed.error);
                        }
                        
                        if (parsed.done) {
                            // Save complete message
                            this.messages.push({ role: 'assistant', content: fullContent });
                            break;
                        }
                        
                        if (parsed.content) {
                            fullContent += parsed.content;
                            contentDiv.textContent = fullContent;
                            this.scrollToBottom();
                        }
                    } catch (e) {
                        console.error('Error parsing SSE data:', e);
                    }
                }
            }
        }
    }
    
    addMessage(role, content) {
        const messageDiv = this.createMessageElement(role, content);
        this.chatMessages.appendChild(messageDiv);
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
        
        if (content) {
            // Simple markdown-like processing
            const processedContent = this.processContent(content);
            contentDiv.innerHTML = processedContent;
        }
        
        messageDiv.appendChild(avatar);
        messageDiv.appendChild(contentDiv);
        
        return messageDiv;
    }
    
    processContent(content) {
        // Basic processing for code blocks and formatting
        let processed = content;
        
        // Code blocks
        processed = processed.replace(/```(\w+)?\n([\s\S]*?)```/g, (match, lang, code) => {
            return `<pre><code>${this.escapeHtml(code.trim())}</code></pre>`;
        });
        
        // Inline code
        processed = processed.replace(/`([^`]+)`/g, (match, code) => {
            return `<code>${this.escapeHtml(code)}</code>`;
        });
        
        // Bold
        processed = processed.replace(/\*\*([^*]+)\*\*/g, '<strong>$1</strong>');
        
        // Italic
        processed = processed.replace(/\*([^*]+)\*/g, '<em>$1</em>');
        
        // Line breaks
        processed = processed.replace(/\n/g, '<br>');
        
        return processed;
    }
    
    escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }
    
    scrollToBottom() {
        this.chatMessages.scrollTop = this.chatMessages.scrollHeight;
    }
}

// Initialize the app when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    new ChatApp();
});
