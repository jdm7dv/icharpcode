del *.res
assemble /F:StringResources.res
assemble /T:lang-de /F:StringResources.de.res
assemble /T:lang-es /F:StringResources.es.res
assemble /T:lang-se /F:StringResources.se.res
assemble /T:lang-goisern /F:StringResources.goisern.res
assemble /T:lang-it /F:StringResources.it.res
assemble /T:lang-pt /F:StringResources.pt.res
assemble /T:lang-nl /F:StringResources.nl.res
assemble /T:lang-cz /F:StringResources.cz.res
assemble /T:lang-fr /F:StringResources.fr.res
assemble /T:lang-pl /F:StringResources.pl.res
resasm *.res
