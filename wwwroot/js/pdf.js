function openPdfInNewWindow(pdfData, filename) {
    // Конвертируем base64 в blob
    const binaryString = window.atob(pdfData);
    const bytes = new Uint8Array(binaryString.length);
    for (let i = 0; i < binaryString.length; i++) {
        bytes[i] = binaryString.charCodeAt(i);
    }
    const blob = new Blob([bytes], { type: 'application/pdf' });

    // Создаем URL для blob
    const url = URL.createObjectURL(blob);

    // Открываем в новом окне
    const newWindow = window.open(url, '_blank');

    if (newWindow) {
        newWindow.focus();
    } else {
        // Если всплывающие окна заблокированы, предлагаем скачать
        const link = document.createElement('a');
        link.href = url;
        link.download = filename || 'document.pdf';
        link.click();
    }
}

function downloadPdf(pdfData, filename) {
    const binaryString = window.atob(pdfData);
    const bytes = new Uint8Array(binaryString.length);
    for (let i = 0; i < binaryString.length; i++) {
        bytes[i] = binaryString.charCodeAt(i);
    }
    const blob = new Blob([bytes], { type: 'application/pdf' });
    const url = URL.createObjectURL(blob);

    const link = document.createElement('a');
    link.href = url;
    link.download = filename || 'document.pdf';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    URL.revokeObjectURL(url);
}