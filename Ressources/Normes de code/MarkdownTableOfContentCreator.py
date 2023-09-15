def CreateTableOfContent(markdownFilePath : str) -> str:
    charsToIgnore = ["'", ",", "#", "\\", "`", "?"]
    
    markdownFile = open(markdownFilePath, "r", encoding="utf-8")
    lines = markdownFile.read().splitlines()

    result = ""

    inCodeBlock = False

    for line in lines:
        if (inCodeBlock):
            if (line.startswith('```')):
                inCodeBlock = False
        else :
            if (line.startswith('```')):
                inCodeBlock = True
            elif (line.startswith('#')):
                i = 0
                while line[i] == '#' and i < len(line) - 1:
                    i += 1
                
                i -= 1
                if (i > 0):
                    titleContent = line[(i + 2):]
                    titleLink = titleContent.lower().replace(" ", "-")
                    for char in charsToIgnore:
                        titleLink = titleLink.replace(char, "")

                    result += f"{' ' * 2 * (i - 1)}- [{titleContent}](#{titleLink})\n"
    
    return result

filePath = "ProjetP3\\Normes.md"

print(CreateTableOfContent(filePath))