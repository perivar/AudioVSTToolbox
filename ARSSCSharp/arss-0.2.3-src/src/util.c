#include "util.h"

inline void test_fft() {

	double signal[256] = {
		0.000865391,
		0.104118064,
		-0.105368771,
		-0.096618481,
		0.274461865,
		0.100624651,
		-0.031682827,
		0.178861931,
		-0.041154884,
		0.0745764,
		-0.056584343,
		-0.011519475,
		0.37244159,
		-0.333296567,
		-0.07670486,
		0.012635638,
		0.06430091,
		0.061967451,
		0.035306752,
		-0.011976154,
		0.013347088,
		-0.015255818,
		-0.032171197,
		-0.421613872,
		0.176026717,
		-0.035100222,
		-0.04386339,
		0.008789533,
		-0.046787836,
		0.010265357,
		0.035256006,
		0.009122181,
		-0.022775872,
		-0.043074373,
		0.002139942,
		0.002891588,
		-0.04839379,
		0.004932872,
		0.009055731,
		0.120602608,
		0.350419849,
		-0.049741529,
		0.018746957,
		-0.020023627,
		-0.018909276,
		0.05838256,
		0.003512445,
		0.032333404,
		-0.004187524,
		0.053402066,
		-0.193661004,
		-0.030811183,
		-0.010904786,
		-0.002647284,
		-0.019114079,
		0.148751348,
		-0.258452237,
		0.19318524,
		-0.024676971,
		-0.08230494,
		0.153271362,
		-0.013393482,
		0.225574851,
		0.066862829,
		-0.005007621,
		0.054133821,
		0.304719567,
		0.035531294,
		-0.074139223,
		0.01723122,
		0.054142788,
		-0.05307924,
		0.027416158,
		0.050989781,
		0.045185816,
		-0.02669557,
		0.020914664,
		0.020171048,
		-0.032324258,
		-0.038908169,
		0.002208921,
		-0.008629396,
		-0.04518985,
		-0.047292139,
		0.029560423,
		-0.011835856,
		-0.003346157,
		-0.174357966,
		0.080544405,
		-0.167937592,
		0.296601146,
		-0.019905306,
		-0.011410435,
		0.008175705,
		0.006888151,
		0.021011366,
		-0.068038754,
		-0.04927912,
		-0.078994833,
		0.031503096,
		0.08944536,
		0.110646628,
		0.144155294,
		0.035049524,
		-0.109210745,
		0.026256427,
		-0.019852556,
		-0.062412113,
		-0.06595698,
		0.27951315,
		0.081969298,
		0.016485604,
		0.055248857,
		-0.021263424,
		0.020316027,
		-0.068477564,
		0.009670784,
		-0.014175341,
		-0.012252999,
		0.013099598,
		0.019670077,
		-0.008961075,
		-0.012455529,
		-0.010228375,
		0.012758554,
		0.008688235,
		0.130880281,
		0.004299275,
		0.002608773,
		-0.038659882,
		-0.035320021,
		0.03946986,
		0.045862138,
		0.025777206,
		0.015493953,
		0.361725807,
		-0.041336037,
		0.065300807,
		-0.006711667,
		0.048016701,
		-0.412198603,
		-0.522793591,
		-0.305609763,
		-0.041047633,
		-0.146241426,
		-0.105980113,
		-0.022788802,
		0.006023734,
		0.038044345,
		0.022009104,
		-0.025985572,
		0.025816889,
		-0.005321069,
		0.045612838,
		0.022013335,
		0.018998574,
		-0.030563755,
		0.014832703,
		-0.014628937,
		-0.009070123,
		-0.02606459,
		-0.013203338,
		-0.359192133,
		0.112673149,
		-0.174063757,
		0.024307109,
		0.048371878,
		-0.109712929,
		-0.028758414,
		0.007553618,
		0.030553628,
		0.010247151,
		0.034802798,
		0.001573765,
		0.009841984,
		-0.009444373,
		-0.030185122,
		-0.009690596,
		-0.310107172,
		0.059100546,
		0.086071484,
		0.014892465,
		-0.045540437,
		-0.449009806,
		-0.018961258,
		-0.081764825,
		0.17429018,
		-0.006051018,
		-0.103804767,
		-0.288634688,
		0.114703834,
		-0.138847873,
		0.09735018,
		0.382123768,
		-0.047125801,
		0.081403479,
		-0.083455987,
		0.074241497,
		0.027118556,
		-0.074722409,
		-0.045518398,
		-0.040908173,
		0.012777518,
		0.038880546,
		0.049085863,
		0.032236215,
		-0.059107684,
		-0.02500126,
		0.00640589,
		0.039520133,
		0.066216588,
		0.03575873,
		0.028480541,
		-0.014959369,
		-0.00399899,
		-0.035790734,
		-0.03992743,
		-0.048084497,
		0.002207351,
		0.030950662,
		-0.320640296,
		0.034640063,
		-0.006563974,
		-0.029881194,
		0.022486171,
		-0.011634802,
		-0.273039907,
		0.48763299,
		0.111061722,
		0.05188515,
		0.095795095,
		0.078814439,
		0.105761215,
		-0.140504986,
		-0.085491545,
		-0.087526135,
		-0.008067675,
		0.033521228,
		0.006883552,
		0.064519316,
		0.003384398,
		-0.014617815,
		0.032730252,
		0.001646112,
		-0.00196098,
		0.012564101,
		-0.304068267,
		-0.267732918,
		-0.209717855,
		-0.06585405,
		-0.00726669,
		-0.037195243,
		-0.074893326,
		-0.012096515,
		0.046475943,
		0.0681374 };

	FILE *file;
	file = fopen("arss-fft-test.csv","a+"); /* append file (add text to a file or create a file if it does not exist.*/

	int i;
	double *my_fft;
	double **out;
	
	int count = 256;
	my_fft=malloc (sizeof(double) * count);	
	
	out = malloc (2 * sizeof(double *));
	out[0] = calloc(count, sizeof(double));	
	out[1] = calloc(count, sizeof(double));	
	
	for (i=0;i<count;i++)
	{
		my_fft[i] = signal[i];
		out[0][i] = signal[i];
	}
	
	// output is returned in halfcomplex format
	fft(my_fft, my_fft, count, 0); //FFT

	for (i=0;i<count;i++)
	{
		out[1][i] = my_fft[i];
	}

	// output is returned in real format
	fft(my_fft, my_fft, count, 1); //IFFT
	
	//fprintf(file, "IN,FFT,IFFT\n");
	for (i=0;i<count;i++)
	{
		fprintf(file, "%d,%.9f,%.9f,%.9f\n", i+1, out[0][i], out[1][i], my_fft[i]/count); 
	}
	fclose(file); 
}
	
inline void log_double_array2D(char *filename, double **s, int32_t xs, int32_t ys) {

	FILE *file;
	file = fopen(filename,"w+"); // clear file if exists

	char buff[50];
	sprintf( buff, "Saving 2D array to file. %d , %d", xs, ys );
	log_file( buff );	

	int32_t ix, iy;

	for (iy=0; iy < ys; iy++)
		for (ix=0; ix < xs; ix++)
			fprintf(file, "[%d,%d] = %.9f\n", iy, ix, s[iy][ix]); 
	
	fclose(file); 
}

inline void log_double_array(char *filename, double *array, int32_t len) {

	FILE *file;
	file = fopen(filename,"w+"); // clear file if exists

	int i;	
	
	// There's no way to know the size of an array if all you have is a pointer; 
	// you must pass an array length into a function.
	int orig_len = len;
	len = 256;		
	if (len > orig_len) {
		len = orig_len;
	}
	log_int32_t("Saving array to file - array length", len);
	
	for (i=0;i<len;i++)
	{
		fprintf(file, "%d,%.9f\n", i+1, array[i]); 
	}
	fclose(file); 
}

inline void log_double_array_placeholder(char *filename_with_placeholder, int32_t filename_counter, double *array, int32_t len) {

	char buff[50];
	sprintf( buff, filename_with_placeholder, filename_counter );
	log_file( buff );	
	
	log_double_array(buff, array, len);
}

inline void log_file(char *str)	
{
	FILE *file;
	file = fopen("log.txt","a+"); /* append file (add text to a file or create a file if it does not exist.*/
	fprintf(file,"%i %s\n", gettime(), str); /*writes*/
	fclose(file); /*done!*/
}

inline void log_double(char *str, double y) 
{
	char buff[50];
	sprintf( buff, "%s = %.9f", str, y );
	log_file( buff );
}

inline void log_int32_t(char *str, int32_t y) 
{
	char buff[50];
	sprintf( buff, "%s = %d", str, y );
	log_file( buff );
}

inline double myfmod(double x, double mod) 
{
	#ifdef DEBUG
	char buff[50];
	sprintf( buff, "myfmod in: [%.9f]", x );
	log_file( buff );
	#endif

	double y = fmod(x, mod);

	#ifdef DEBUG
	sprintf( buff, "myfmod out: [%.9f]", y );
	log_file( buff );
	#endif
	
	return y;
}

void win_return()
{
	#ifdef WIN32
	if (quiet==0)
	{
		printf("Press Return to quit\n");
		getchar();
	}
	#endif
}

#ifdef WIN32
#include "Windows.h"

int32_t gettime()	// in milliseconds
{
	return (int32_t) GetTickCount();
}
#else
#include <sys/time.h>

int32_t gettime()	// in milliseconds
{
	struct timeval t;

	gettimeofday(&t, NULL);

	return (int32_t) t.tv_sec*1000 + t.tv_usec/1000;
}
#endif

inline double roundoff(double x)	// nearbyint() replacement, with the exception that the result contains a non-zero fractional part
{
	#ifdef DEBUG
	char buff[50];
	sprintf( buff, "roundoff in: [%.9f]", x );
	log_file( buff );
	#endif

	double y;
	
	if (x>0)
		y = x + 0.5;
	else
		y = x - 0.5;

	#ifdef DEBUG
	sprintf( buff, "roundoff out: [%.9f]", y );
	log_file( buff );
	#endif
		
	return y;
}

inline int32_t roundup(double x)
{
	#ifdef DEBUG
	char buff[50];
	sprintf( buff, "roundup in: [%.9f]", x );
	log_file( buff );
	#endif

	int32_t y;
	
	if (myfmod(x, 1.0) == 0)
		y = (int32_t) x;
	else
		y = (int32_t) x + 1;

	#ifdef DEBUG
	sprintf( buff, "roundup out: [%d]", y );
	log_file( buff );
	#endif
		
	return y;
}

float getfloat()
{
	float x;
	char a[32];
	fgets(a, 31, stdin);
	if (a[0]==0)
		return 0.0;
	else
	{
		x=atof(a);
		return x;
	}
}

inline int32_t smallprimes(int32_t x)	// returns 1 if x is only made of these small primes
{
	int32_t i, p[2]={2, 3};

	for (i=0; i<2; i++)
		while (x%p[i] == 0)
			x/=p[i];

	return x;
}

inline int32_t nextsprime(int32_t x)	// returns the next integer only made of small primes
{
	#ifdef DEBUG
	char buff[50];
	sprintf( buff, "nextsprime in: [%d]", x );
	log_file( buff );
	#endif

	while (smallprimes(x)!=1)
		x++;

	#ifdef DEBUG
	sprintf( buff, "nextsprime out: [%d]", x );
	log_file( buff );
	#endif
		
	return x;
}

inline double log_b(double x)
{
	#ifdef DEBUG
	char buff[50];
	sprintf( buff, "log_b in: [%.9f]", x );
	log_file( buff );
	#endif

	double y;
	
	if (LOGBASE==1.0)
		y = x;
	else
		#ifdef DEBUG
		if (x==0)
			fprintf(stderr, "Warning : log(0) returning -infinite\n");
		else
		#endif
		
	y = log(x)/log(LOGBASE);
	
	#ifdef DEBUG
	sprintf( buff, "log_b out: [%.9f]", y );
	log_file( buff );
	#endif
	
	return y;
}

inline uint32_t rand_u32()
{
	#if RAND_MAX == 2147483647
		return rand();
	#elif RAND_MAX == 32767
		return ((rand()%256)<<24) | ((rand()%256)<<16) | ((rand()%256)<<8) | (rand()%256);
	#else
		fprintf(stderr, "Unhandled RAND_MAX value : %d\nPlease signal this error to the developer.", RAND_MAX);
		return rand();
	#endif
}

inline double dblrand()	// range is +/- 1.0
{
	double y = ((double) rand_u32() * (1.0 / 2147483648.0)) - 1.0;
	
	char buff[50];
	sprintf( buff, "dblrand out: [%.9f]", y );
	log_file( buff );
	
	return y;
}

inline uint16_t fread_le_short(FILE *file)		// read from file a 16-bit integer in little endian
{
	uint8_t	byte_a, byte_b;

	fread(&byte_a, 1, 1, file);
	fread(&byte_b, 1, 1, file);

	return (uint16_t) (byte_b<<8) | byte_a;
}

inline uint32_t fread_le_word(FILE *file)		// read from file a 32-bit integer in little endian
{
	uint8_t	byte_a, byte_b, byte_c, byte_d;

	fread(&byte_a, 1, 1, file);
	fread(&byte_b, 1, 1, file);
	fread(&byte_c, 1, 1, file);
	fread(&byte_d, 1, 1, file);

	return (uint32_t) (byte_d<<24) | (byte_c<<16) | (byte_b<<8) | byte_a;
}

inline void fwrite_le_short(uint16_t s, FILE *file)	// write to file a 16-bit integer in little endian
{
	uint8_t byte;

	byte = s & 0xFF;
	fwrite(&byte, 1, 1, file);
	byte = (s>>8) & 0xFF;
	fwrite(&byte, 1, 1, file);
}

inline void fwrite_le_word(uint32_t w, FILE *file)	// write to file a 32-bit integer in little endian
{
	uint8_t byte;

	byte = w & 0xFF;
	fwrite(&byte, 1, 1, file);
	byte = (w>>8) & 0xFF;
	fwrite(&byte, 1, 1, file);
	byte = (w>>16) & 0xFF;
	fwrite(&byte, 1, 1, file);
	byte = (w>>24) & 0xFF;
	fwrite(&byte, 1, 1, file);
}

char *getstring()
{
	signed long len_str, i;
	char a[FILENAME_MAX], *b;

	fgets(a, sizeof(a), stdin);
	len_str=strlen(a);

	b=malloc(len_str * sizeof(char));

	for (i=0; i<len_str; i++)
		b[i]=a[i];

	b[len_str-1]=0;

	return b;
}

int32_t str_isnumber(char *string)	// returns 1 is string is a valid float number, 0 otherwise
{
	int32_t i;
	int32_t size = strlen(string);
	char c;

	if (size==0)
		return 0;

	c = string[0];
	if (! ((c>='0' && c<='9') || c=='+' || c=='-'))
		return 0;

	for (i=1; i<size; i++)
	{
		c = string[i];

		if (! ((c>='0' && c<='9') || c=='.' || c=='e'))
			return 0;
	}

	return 1;
}
