function scrollToLatest() {
    document.querySelector('#chat-container').scroll({
        top: Number.MAX_SAFE_INTEGER,
        behavior: 'smooth'
    })
}
