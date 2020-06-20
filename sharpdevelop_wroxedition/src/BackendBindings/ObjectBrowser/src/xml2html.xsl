<?xml version="1.0" encoding="iso-8859-1" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	
	<xsl:variable name="indent" select="'&#160;&#160;&#160;&#160;'" />

	<xsl:template match="/assembly">
		<html>
			<head>
				<title>
					<xsl:value-of select="./@name" />
				</title>
				<style type="text/css">
					<xsl:text>
						body {
							font-family: Courier New;
							font-size: 10pt;
						}
						.space {
							font-size: 9pt;
						}
					</xsl:text>
				</style>
			</head>
			<body>
				<xsl:apply-templates />
			</body>
		</html>
	</xsl:template>

	<xsl:template match="class">
		<img src="gx/class.gif" alt="class" />
		<span class="space">&#160;</span>
		<xsl:value-of select="./@name" />
		<br />
		<xsl:apply-templates />
	</xsl:template>

	<xsl:template match="event">
		<xsl:value-of select="$indent" />
		<img src="gx/event.gif" alt="event" />
		<span class="space">&#160;</span>
		<xsl:value-of select="./@name" />
		<br />
	</xsl:template>

	<xsl:template match="property">
		<xsl:value-of select="$indent" />
		<img src="gx/property.gif" alt="property" />
		<span class="space">&#160;</span>
		<xsl:value-of select="./@name" />
		<xsl:text> : </xsl:text>
		<xsl:value-of select="./@type" />
		<br />
	</xsl:template>

	<xsl:template match="method">
		<xsl:value-of select="$indent" />
		<img src="gx/method.gif" alt="method" />
		<span class="space">&#160;</span>
		<xsl:value-of select="./@name" />
		<xsl:text> : </xsl:text>
		<xsl:value-of select="./@type" />
		<xsl:text>(</xsl:text>
		<xsl:variable name="count" select="count(./param)" />
		<xsl:for-each select="param">
			<xsl:variable name="parampos" select="position()" />
			<xsl:choose>
				<xsl:when test="$parampos != $count">
					<xsl:value-of select="./@type" />
					<xsl:text> </xsl:text>
					<xsl:value-of select="./@name" />
					<xsl:text>, </xsl:text>
				</xsl:when>
				<xsl:otherwise>
					<xsl:value-of select="./@type" />
					<xsl:text> </xsl:text>
					<xsl:value-of select="./@name" />
				</xsl:otherwise>
			</xsl:choose>
		</xsl:for-each>
		<xsl:text>)</xsl:text>
		<br />
	</xsl:template>

	<xsl:template match="field">
		<xsl:value-of select="$indent" />
		<img src="gx/field.gif" alt="field" />
		<span class="space">&#160;</span>
		<xsl:value-of select="./@name" />
		<xsl:text> : </xsl:text>
		<xsl:value-of select="./@type" />
		<br />
	</xsl:template>
</xsl:stylesheet>
