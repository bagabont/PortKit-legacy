<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:output method="html"/>

  <xsl:template match="/">
    <html>
      <head>
        <link rel="stylesheet" href="LogStyle.css" />
      </head>
      <body>
        <h2>Log</h2>
        <table border="1">
          <tr>
            <th>Timestamp</th>
            <th>Caller</th>
          </tr>
          <xsl:for-each select="session/log">
            <tr>
              <xsl:attribute name="class">
                <xsl:value-of select="@level"/>
              </xsl:attribute>
              <td>
                <xsl:value-of select="@timestamp" />
              </td>
              <td>
                <xsl:value-of select="@caller"/>
              </td>
              <td>
                <xsl:value-of select="."/>
              </td>
            </tr>
          </xsl:for-each>
        </table>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>
