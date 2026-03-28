import PyPDF2

pdf_path = r'C:\Project\System Test Task Assignment-1.pdf'
try:
    with open(pdf_path, 'rb') as file:
        reader = PyPDF2.PdfReader(file)
        text = ""
        for page_num in range(len(reader.pages)):
            page = reader.pages[page_num]
            text += f"\n--- PAGE {page_num + 1} ---\n"
            text += page.extract_text()
        print(text)
except Exception as e:
    print(f"Error: {e}")
